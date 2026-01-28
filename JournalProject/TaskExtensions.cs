namespace JournalProject;

public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
        task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                System.Diagnostics.Debug.WriteLine($"FireAndForget task failed: {t.Exception}");
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
