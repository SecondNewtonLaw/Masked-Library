using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Masked.Logging;

/// <summary>
/// Class that provides logging utilities.
/// </summary>
internal sealed class Logger
{
    private WorkerStatus LoggerStatus;
    private readonly static string _logFolder = $"{Environment.CurrentDirectory}/Logs/";
    private readonly static string _logPath = $"{_logFolder}log.log";
    private readonly Thread _worker = new(() => Logger.Shared.WorkerCode());
    internal static Logger Shared { get; } = new Logger();
    private readonly List<Action> PendingIOs = new();

    public static WorkerStatus GetLoggerWorkerStatus()
        => Shared.LoggerStatus;
    public static string GetLogPath()
        => _logFolder;
    public static async Task LogToFile(string logText, LogLevel logLevel, dynamic invoker, Thread origin, CancellationToken token = new())
    {
        // Start Logger Worker Thread if not initialized.
        if (Shared._worker.ThreadState is ThreadState.Unstarted)
            Shared._worker.Start();

        await Task.Run(() =>
        {
            Shared.PendingIOs.Add(() => Task.Run(async () =>
            {
                // If it is a System.Object, get the type, else print it as a string.
                dynamic inner = invoker.ToString().Contains("System.Object") ? invoker.GetType() : invoker.ToString();

                StreamWriter swrite = File.AppendText(_logPath);
                await swrite.WriteLineAsync(
                    string.Format("[Thread N{0}] [Origin: {1}] [{2}] PID: {3}: {4}", origin.ManagedThreadId, inner, GetLogLevel(logLevel), Environment.ProcessId, logText.Split('\n')[^1].ReplaceLineEndings(""))
                ).ConfigureAwait(continueOnCapturedContext: false);
                await swrite.FlushAsync().ConfigureAwait(continueOnCapturedContext: false);
                swrite.Close();
                await swrite.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
            }, token));
        }, token).ConfigureAwait(continueOnCapturedContext: false);
    }
    private void WorkerCode()
    {
        // Create Logs folder if not exists
        if (!Directory.Exists(_logFolder))
            Directory.CreateDirectory(_logFolder);

        _worker.Name = "Logger Thread";
        _worker.IsBackground = true;
        while (true)
        {
            this.LoggerStatus = WorkerStatus.Working;
            WorkerLoop();
            this.LoggerStatus = WorkerStatus.Idle;
            //// Thread.Sleep(System.Random.Shared.Next(0, 2) * 1000);
        }
    }
    private void WorkerLoop()
    {
        for (int i = 0; i < PendingIOs.Count; i++)
        {
            PendingIOs[i]?.Invoke();
            PendingIOs.RemoveAt(i);
        }
    }
    private static string GetLogLevel(LogLevel lvl)
    {
        if (lvl is LogLevel.Information)
            return "I";

        if (lvl is LogLevel.Warning)
            return "W";

        if (lvl is LogLevel.Error)
            return "E";

        return (lvl is LogLevel.Verbose) ? "V" : "D";
    }
    // Private Ctor
    private Logger()
    { }
}