using Masked.Sys.Extensions;

namespace Masked.Sys.IO.Extensions;

public static class StreamExt
{
    /// <summary>
    /// Flushes, Disposes and closes a collection of <see cref="ICollection"/> streams.
    /// </summary>
    /// <param name="inStreams">Streams to destroy</param>
    public static async Task DestroyStreamsAsync(this IEnumerable<Stream> inStreams, TaskScheduler? sched = null, CancellationToken tken = default)
    {
        sched ??= TaskScheduler.Current;

        List<Stream> enumerated = inStreams.ToList();
        List<Task> taskList = new(enumerated.Count);

        enumerated.FastIterator((stream, index) =>
        {
            int i2 = index;
            taskList.Add(Task.Factory.StartNew(async () =>
            {
                await stream.FlushAsync();
                await stream.DisposeAsync();
                stream.Close();
            }, tken, TaskCreationOptions.HideScheduler, sched));
        });

        while (taskList.Count > 0)
        {
            Task completed = await Task.WhenAny(taskList); // Completed
            _ = taskList.Remove(completed); // Remove completed.
        }
    }
}