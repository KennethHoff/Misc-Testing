using System.Runtime.CompilerServices;

namespace KHtmx.Utilities.GlobalExtensions;

public static class TaskExtensions
{
    // T1, T2
    public static TaskAwaiter<(T1, T2)> GetAwaiter<T1, T2>(this (Task<T1>, Task<T2>) tasks)
    {
        return CombineTasks().GetAwaiter();
        async Task<(T1, T2)> CombineTasks()
        {
            var (t1, t2) = tasks;
            await Task.WhenAll(t1, t2);
            return (t1.Result, t2.Result);
        }
    }

    // T1, T2, T3
    public static TaskAwaiter<(T1, T2, T3)> GetAwaiter<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks)
    {
        return CombineTasks().GetAwaiter();
        async Task<(T1, T2, T3)> CombineTasks()
        {
            var (t1, t2, t3) = tasks;
            await Task.WhenAll(t1, t2, t3);
            return (t1.Result, t2.Result, t3.Result);
        }
    }
}
