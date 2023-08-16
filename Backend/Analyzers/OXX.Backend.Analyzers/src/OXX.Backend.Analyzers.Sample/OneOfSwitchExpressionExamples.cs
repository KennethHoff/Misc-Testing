using OneOf;

Hmm.MethodThatThrows();

public static class Hmm
{
    public static void MethodThatThrows()
    {
        throw new ArgumentException();
    }

    public static void MethodThatDoesNotThrow()
    {
        OneOf<string, bool>? hmm = "hmm";
        var huh = hmm ?? throw new Exception();

        try
        {
            var message = hmm.Value switch
            {
                string x => "hmm",
                bool x => throw new NotImplementedException(),
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        throw new AggregateException();
    }
}
