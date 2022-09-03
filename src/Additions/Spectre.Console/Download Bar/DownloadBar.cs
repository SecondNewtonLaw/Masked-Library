using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace Masked.SpectreConsole;

public class DownloadBar
{
    public HttpClient _internalHttpClient = null!;
    public DownloadBar()
    {
        _internalHttpClient = new(new HttpClientHandler()
        {
            SslProtocols = SslProtocols.Tls12,
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.All,
        });
    }
    public DownloadBar(HttpClient httpClient) { _internalHttpClient = httpClient; }
    public async Task<IDictionary<Stream, DownloadBarItem>> StartDownloadBar(IEnumerable<DownloadBarItem> urlList)
    {
        Dictionary<HttpResponseMessage, DownloadBarItem> streamTracker = new();

        for (int i = 0; urlList.Skip(i).Any(); i++)
        {
            int iter = i;
            streamTracker.Add(
                await _internalHttpClient.GetAsync(urlList.ElementAt(iter).Url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false),
                    urlList.ElementAt(i)
                );
        }

        Dictionary<Stream, DownloadBarItem> FinalizedDownloads = new();

        await AnsiConsole.Progress()
            .AutoRefresh(enabled: true) // Turn off auto refresh
            .AutoClear(enabled: false)   // Do not remove the task list when done
            .HideCompleted(enabled: false)   // Hide tasks as they are completed
            .StartAsync(async ctx =>
            {
                List<ProgressTask> progTasks = new();
                List<Thread> threadList = new();

                const int chunkSize = 32768; // The size of each chunk that shall be read on each iteration every O(n)

                for (int i = 0; i < streamTracker.Count; i++)
                {
                    int currIterator = i;
                    string dlName = urlList.ElementAt(i).ItemName.RemoveMarkup();
                    double contentLength = (double)streamTracker.ElementAt(i).Key.Content.Headers.ContentLength!;
                    progTasks.Add(
                        ctx.AddTask(
                            description: $"[green]Downloading [yellow bold]{dlName}[/]...[/]",
                            autoStart: true,
                            maxValue: contentLength
                        )
                    );

                    Thread thread = new(async () =>
                    {
                        Stream httpStream = await streamTracker.ElementAt(currIterator).Key.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        Stream finished = File.Create(streamTracker.ElementAt(currIterator).Value.SavePath);
                        int readData = 0, lastRead = 0;
                        byte[] buffer = new byte[chunkSize];

                        do
                        {
                            lastRead = await httpStream.ReadAsync(buffer.AsMemory(0, chunkSize)).ConfigureAwait(false);

                            await finished.WriteAsync(buffer.AsMemory(0, lastRead)).ConfigureAwait(false);

                            readData += lastRead;

                            progTasks[currIterator].Value = readData;
                        }
                        while (readData < contentLength);

                        await finished.FlushAsync().ConfigureAwait(false); // Save all data to file

                        FinalizedDownloads.Add(finished, urlList.ElementAt(currIterator));
                    });
                    thread.Start();
                    threadList.Add(thread);
                }
                while (!ctx.IsFinished)
                {
                    // 'Pseudo-Code': 
                    // Using Multi-Threading, 
                    // update each task simultaneously and then return them to a stream, 
                    // to do it, run EVERY thread separately in their own drill,
                    // the while is only used to retain the progress of ending..
                    await Task.Delay(100).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

        return FinalizedDownloads;
    }
}