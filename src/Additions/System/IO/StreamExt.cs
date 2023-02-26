using Masked.Sys.Extensions;

namespace Masked.Sys.IO.Extensions;

/// <summary>
/// Extensions for the Stream class.
/// </summary>
public static class StreamExt {
    /// <summary>
    /// Flushes, Disposes and closes a collection of <see cref="IEnumerable{T}"/> streams.
    /// </summary>
    /// <param name="inStreams">Streams to destroy</param>
    public static async Task DestroyStreamsAsync(this IEnumerable<Stream> inStreams, TaskScheduler? sched = null,
        CancellationToken tken = default) {
        sched ??= TaskScheduler.Current;

        List<Stream> enumerated = inStreams.ToList();
        List<Task> taskList = new(enumerated.Count);

        enumerated.FastIterator((stream, index) => {
            var i2 = index;
            taskList.Add(Task.Factory.StartNew(async () => {
                await stream.FlushAsync();
                await stream.DisposeAsync();
                stream.Close();
            }, tken, TaskCreationOptions.HideScheduler, sched));
            return NextStep.Continue;
        });

        while (taskList.Count > 0) {
            var completed = await Task.WhenAny(taskList); // Completed
            _ = taskList.Remove(completed); // Remove completed.
        }
    }
}