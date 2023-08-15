// using OneOf;
//
// OneOf<string, bool> twoOf = "hmm";
//
// var message = twoOf.Value switch
// {
//     bool x => "lol",
//     string x => throw new NotImplementedException()
// };

Hmm.MethodThatThrows();
return;


public static class Hmm
{
    public static void MethodThatThrows()
    {
        throw new ArgumentException();
    }
}

