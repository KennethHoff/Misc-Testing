using KH.Orleans.GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace KH.Orleans.Grains;

public sealed class HelloWorldGrain(ILogger<HelloWorldGrain> logger) : Grain, IHelloWorldGrain
{
    private int _counter;

    public ValueTask<string> SayHello(string name)
    {
        // Get the int Id of the current grain
        var id = this.GetPrimaryKeyLong();
        logger.LogInformation("SayHello message received: greeting = '{Name}', from grain with Id = {Id}", name, id);

        _counter++;

        var counterString = _counter switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            _ => $"{_counter}th",
        };
        return new ValueTask<string>($"Hello, {name}! This is the {counterString} time you've said hello.");
    }
}
