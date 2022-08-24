using System.IO;

namespace Masked.Sys.IO.Extensions;

public static class StreamExt
{
    /// <summary>
    /// Flushes, Disposes and closes a collection of <see cref="ICollection"/> streams.
    /// </summary>
    /// <param name="inStreams">Streams to destroy</param>
    public static async Task DestroyStreamsAsync(this ICollection<Stream> inStreams, TaskScheduler sched = null!, CancellationToken tken = new())
    {
        if (sched is null)
            sched = TaskScheduler.Current;

        List<Task> taskList = new();
        for (int i = 0; i < inStreams.Count; i++)
        {
            int i2 = i;
            taskList.Add(Task.Factory.StartNew(async () =>
            {
                await inStreams.ElementAt(i2).FlushAsync();
                await inStreams.ElementAt(i2).DisposeAsync();
                inStreams.ElementAt(i2).Close();
            }, tken, TaskCreationOptions.HideScheduler, sched));
        }

        while (taskList.Count > 0)
        {
            Task completed = await Task.WhenAny(taskList); // Completed
            taskList.Remove(completed); // Remove completed.
        }
    }
}