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
            AutomaticDecompression = DecompressionMethods.Brotli
        });
    }
    public DownloadBar(HttpClient httpClient) { _internalHttpClient = httpClient; }
    public async Task<Dictionary<Stream, DownloadBarItem>> StartDownloadBar(List<DownloadBarItem> urlList)
    {
        Dictionary<HttpResponseMessage, DownloadBarItem> streamTracker = new();

        for (int i = 0; i < urlList.Count; i++)
        {
            int iter = i;
            streamTracker.Add(
                await _internalHttpClient.GetAsync(urlList[iter].url, HttpCompletionOption.ResponseHeadersRead),
                    urlList[i]
                );
        }

        Dictionary<Stream, DownloadBarItem> FinalizedDownloads = new();

        await AnsiConsole.Progress()
            .AutoRefresh(true) // Turn off auto refresh
            .AutoClear(false)   // Do not remove the task list when done
            .HideCompleted(false)   // Hide tasks as they are completed
            .StartAsync(async ctx =>
            {
                List<ProgressTask> progTasks = new();
                List<Thread> threadList = new();

                const int chunkSize = 32768; // The size of each chunk that shall be read on each iteration every O(n)

                for (int i = 0; i < streamTracker.Count; i++)
                {
                    int currIterator = i;
                    string dlName = urlList[i].itemName.RemoveMarkup();
                    double contentLength = (double)streamTracker.ElementAt(i).Key.Content.Headers.ContentLength!;
                    progTasks.Add(
                        ctx.AddTask(
                            $"[green]Downloading [yellow bold]{dlName}[/]...[/]",
                            true,
                            contentLength
                        )
                    );

                    Thread thread = new(async () =>
                    {
                        Stream httpStream = await streamTracker.ElementAt(currIterator).Key.Content.ReadAsStreamAsync();
                        Stream finished = File.Create(streamTracker.ElementAt(currIterator).Value.savePath);
                        int readData = 0, lastRead = 0;
                        byte[] buffer = new byte[chunkSize];

                        do
                        {
                            lastRead = await httpStream.ReadAsync(buffer.AsMemory(0, chunkSize));

                            await finished.WriteAsync(buffer.AsMemory(0, lastRead));

                            readData += lastRead;

                            progTasks[currIterator].Value = readData;
                        }
                        while (readData < contentLength);

                        await finished.FlushAsync(); // Save all data to file

                        FinalizedDownloads.Add(finished, urlList[currIterator]);
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
                    await Task.Delay(100);
                }
            });

        return FinalizedDownloads;
    }
}
public struct DownloadBarItem
{
    public DownloadBarItem(Uri url, string itemName, string savePath) { this.url = url; this.itemName = itemName; this.savePath = savePath; }
    public Uri url { get; init; }
    public string itemName { get; init; }
    public string savePath { get; init; }
}