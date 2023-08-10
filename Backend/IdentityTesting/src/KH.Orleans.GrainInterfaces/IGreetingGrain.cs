namespace KH.Orleans.GrainInterfaces;

public interface IGreetingGrain : IGrainWithStringKey
{
    ValueTask<string> Greet();
}
