using KH.Orleans.GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace KH.Orleans.Grains;

public sealed class GreetingGrain(ILogger<GreetingGrain> logger) : Grain, IGreetingGrain
{
    private int _counter;

    public ValueTask<string> Greet()
    {
        // Get the int Id of the current grain
        var id = this.GetPrimaryKeyString();
        logger.LogInformation("Greetings from grain with Id {Id}", id);

        _counter++;

        var counterString = _counter switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            _ => $"{_counter}th",
        };
        return new ValueTask<string>($"Hello, {id}! This is the {counterString} time you've said hello.");
    }
}
