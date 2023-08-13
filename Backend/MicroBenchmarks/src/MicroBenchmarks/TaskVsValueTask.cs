using BenchmarkDotNet.Attributes;

namespace MicroBenchmarks;

[MemoryDiagnoser(false)]
public class TaskVsValueTask
{
    [Benchmark]
    public async Task<int> Task_AwaitCompleted()
    {
        await Task.CompletedTask;
        return 42;
    }
    
    [Benchmark]
    public Task<int> Task_ReturnCompleted()
    {
        return Task.FromResult(42);
    }
    
    [Benchmark]
    public async Task<int> Task_AwaitNothing()
    {
        return 42;
    }

    [Benchmark]
    public async Task<int> Task_AwaitATask()
    {
        await new Task<int>(() => 42);
        return 42;
    }
    
    [Benchmark]
    public async Task<int> Task_AwaitAValueTask()
    {
        await new ValueTask<int>(42);
        return 42;
    }

    [Benchmark]
    public async ValueTask<int> ValueTask_AwaitCompleted()
    {
        await ValueTask.CompletedTask;
        return 42;
    }
    
    [Benchmark]
    public ValueTask<int> ValueTask_ReturnCompleted()
    {
        return new ValueTask<int>(42);
    }
    
    [Benchmark]
    public async ValueTask<int> ValueTask_AwaitNothing()
    {
        return 42;
    }
    
    [Benchmark]
    public async ValueTask<int> ValueTask_AwaitATask()
    {
        await new Task<int>(() => 42);
        return 42;
    }
    
    [Benchmark]
    public async ValueTask<int> ValueTask_AwaitAValueTask()
    {
        await new ValueTask<int>(42);
        return 42;
    }
}
