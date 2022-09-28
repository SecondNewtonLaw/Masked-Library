using System.Net;
using System.Security.Authentication;

using Spectre.Console;

namespace Masked.SpectreConsole;

public sealed class DownloadBar
{
    public HttpClient _internalHttpClient = null!;
    private const int chunkSize = 32768; // The size of each chunk that shall be read on each iteration every O(n)

    public DownloadBar() => _internalHttpClient = new(new HttpClientHandler()
    {
        SslProtocols = SslProtocols.Tls12,
        UseCookies = false,
        AutomaticDecompression = DecompressionMethods.All,
    });

    public DownloadBar(HttpClient httpClient) => _internalHttpClient = httpClient;

    public async Task<IDictionary<Stream, DownloadBarItem>> StartDownloadBar(IEnumerable<DownloadBarItem> urlList, CancellationToken token = default)
    {
        int urlCount =urlList.Count();
        urlList = urlList.ToList(); // Enumerate all objects, avoids deferring.
        Dictionary<HttpResponseMessage, DownloadBarItem> streamTracker = new(urlCount);

        for (int i = 0; urlList.Skip(i).Any(); i++)
        {
            int iter = i;
            streamTracker.Add(
                await _internalHttpClient.GetAsync(urlList.ElementAt(iter).Url, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false),
                    urlList.ElementAt(i)
                );
        }

        Dictionary<Stream, DownloadBarItem> FinalizedDownloads = new(urlCount);

        await AnsiConsole.Progress()
            .AutoRefresh(enabled: true) // Turn off auto refresh
            .AutoClear(enabled: false)   // Do not remove the task list when done
            .HideCompleted(enabled: false)   // Hide tasks as they are completed
            .StartAsync(async ctx =>
            {
                ProgressTask[] progTasks = new ProgressTask[streamTracker.Count];
                Thread[] threadList = new Thread[streamTracker.Count];

                for (int i = 0; i < streamTracker.Count; i++)
                {
                    int currIterator = i;
                    string dlName = urlList.ElementAt(i).ItemName.RemoveMarkup();
                    double contentLength = (double)streamTracker.ElementAt(i).Key.Content.Headers.ContentLength!;
                    progTasks[i] =
                        ctx.AddTask(
                            description: $"[green]Downloading [yellow bold]{dlName}[/]...[/]",
                            autoStart: true,
                            maxValue: contentLength
                        );

                    Thread thread = new(() => ProgressUpdateCode(currIterator, contentLength, progTasks, FinalizedDownloads, streamTracker, urlList, token).GetAwaiter().GetResult());
                    thread.Start();
                    threadList[i] = thread;
                }

                while (!ctx.IsFinished)
                {
                    // 'Pseudo-Code':
                    // Using Multi-Threading,
                    // update each task simultaneously and then return them to a stream,
                    // to do it, run EVERY thread separately in their own drill,
                    // the while is only used to retain the progress of ending.
                    await Task.Delay(100).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

        return FinalizedDownloads;
    }

    // I did this to avoid a possible program-ending exception.
    private static async Task ProgressUpdateCode(
        int index,
        double lengthOfContent,
        ProgressTask[] tasksTracker,
        Dictionary<Stream, DownloadBarItem> FinalizedDownloads,
        Dictionary<HttpResponseMessage, DownloadBarItem> streamTracker,
        IEnumerable<DownloadBarItem> urlList,
        CancellationToken token)
    {
        Stream httpStream = await streamTracker.ElementAt(index).Key.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
        FileStream finished = File.Create(streamTracker.ElementAt(index).Value.SavePath);
        int readData = 0, lastRead;
        byte[] buffer = new byte[chunkSize];

        do
        {
            lastRead = await httpStream.ReadAsync(buffer.AsMemory(0, chunkSize), token).ConfigureAwait(false);

            await finished.WriteAsync(buffer.AsMemory(0, lastRead), token).ConfigureAwait(false);

            readData += lastRead;

            tasksTracker[index].Value = readData;
        }
        while (readData < lengthOfContent);

        await finished.FlushAsync(token).ConfigureAwait(false); // Save all data to file

        FinalizedDownloads.Add(finished, urlList.ElementAt(index));
    }
}