namespace KH.Orleans.GrainInterfaces;

public interface IHelloWorldGrain : IGrainWithIntegerKey
{
    ValueTask<string> SayHello(string name);
}
