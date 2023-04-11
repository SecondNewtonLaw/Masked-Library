namespace Masked.Logging;

/// <summary>
/// Class that provides logging utilities.
/// </summary>
[Obsolete("Do not use. This class is obsolete and will be removed in a future version due to it being virtually useless and badly written.")]
internal sealed class Logger {
    private WorkerStatus LoggerStatus;
    private static readonly string _logFolder = $"{Environment.CurrentDirectory}/Logs/";
    private static readonly string _logPath = $"{_logFolder}log.log";
    private readonly Thread _worker = new(() => Shared.WorkerCode());
    public static Logger Shared { get; } = new();
    private readonly List<Action> PendingIOs = new();

    public static WorkerStatus GetLoggerWorkerStatus() {
        lock (Shared) {
            return Shared.LoggerStatus;
        };
    }
    
    public static string GetLogPath() {
        return _logFolder;
    }

    public static Task LogToFile(string logText, LogLevel logLevel, dynamic invoker, Thread origin,
        CancellationToken token = new()) {
        // Start Logger Worker Thread if not initialized.
        if (Shared._worker.ThreadState is ThreadState.Unstarted)
            Shared._worker.Start();

        return Task.Run(
            () => Shared.PendingIOs.Add(
                () => Task.Run(async () => {
                    // If it is a System.Object, get the type, else print it as a string.
                    var inner = invoker.ToString().Contains("System.Object")
                        ? invoker.GetType().FullName
                        : invoker.ToString();

                    var swrite = File.AppendText(_logPath);
                    await swrite.WriteLineAsync(
                        $"[Thread N{origin.ManagedThreadId}] [Origin: {inner}] [{GetLogLevel(logLevel)}] PID: {Environment.ProcessId}: {logText.Split('\n')[^1].ReplaceLineEndings("")}"
                    ).ConfigureAwait(false);
                    await swrite.FlushAsync().ConfigureAwait(false);
                    swrite.Close();
                    await swrite.DisposeAsync().ConfigureAwait(false);
                }, token)), token);
    }

    private void WorkerCode() {
        // Create Logs folder if not exists
        if (!Directory.Exists(_logFolder))
            _ = Directory.CreateDirectory(_logFolder);

        _worker.Name = "Logger Thread";
        _worker.IsBackground = true;
        while (true) {
            LoggerStatus = WorkerStatus.Working;
            WorkerLoop();
            LoggerStatus = WorkerStatus.Idle;
            //// Thread.Sleep(System.Random.Shared.Next(0, 2) * 1000);
        }
    }

    private void WorkerLoop() {
        for (var i = 0; i < PendingIOs.Count; i++) {
            PendingIOs[i]?.Invoke();
            PendingIOs.RemoveAt(i);
        }
    }

    private static string GetLogLevel(LogLevel lvl) {
        return lvl switch {
            LogLevel.Information => "I",
            LogLevel.Warning => "W",
            LogLevel.Error => "E",
            LogLevel.Verbose => "V",
            LogLevel.Debug => "D",
            _ => throw new ArgumentException("The value inserted does not match to a known LogLevel.")
        };
    }

    // Private Ctor
    private Logger() { }
}