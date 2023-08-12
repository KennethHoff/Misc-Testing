// JetBrains Rider currently does not support Roslyn Diagnostic Suppressors (https://youtrack.jetbrains.com/issue/RIDER-45021)
// Hence, we need to disable the warning for the entire file as a pretend suppressor.
// This is not necessary for the analyzer to work, it is just to avoid the warning in the IDE.
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
using OneOf;






const string randomString = "hey";
OneOf<bool, string> twoOf = "hmm";

var message = twoOf.Value switch
{
	bool x => "Hey",
	string x => throw new NotImplementedException()
};

var hmm = randomString switch
{
	_ => "throw new NotImplementedException()",
};






#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

