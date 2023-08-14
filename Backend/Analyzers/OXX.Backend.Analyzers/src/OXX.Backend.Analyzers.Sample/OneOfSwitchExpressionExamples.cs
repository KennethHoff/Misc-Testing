using OneOf;

OneOf<string, bool> twoOf = "hmm";

var message = twoOf.Value switch
{
    bool x => "lol",
    string x => throw new NotImplementedException()
};
