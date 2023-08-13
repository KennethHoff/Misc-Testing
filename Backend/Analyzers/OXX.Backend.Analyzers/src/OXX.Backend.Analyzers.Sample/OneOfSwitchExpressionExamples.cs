// JetBrains Rider currently does not support Roslyn Diagnostic Suppressors (https://youtrack.jetbrains.com/issue/RIDER-45021)
// Hence, we need to disable the warning for the entire file as a pretend suppressor.
// This is not necessary for the analyzer to work, it is just to avoid the warning in the IDE.
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
using System.Xml;
using OneOf;

OneOf<string, bool, Program> twoOf = "hmm";

var message = twoOf.Value switch
{
	"Hello" => "Hello!", // Should warn
	true => "yo!", // Should warn
	string x => "yo!", // Should not warn
	bool x => throw new NotImplementedException(), // Should not warn
	Program => "hmm", // Should not warn
	int x => throw new NotImplementedException(), // Should warn
	XmlAttribute x => throw new NotImplementedException(), // Should warn
	_ => throw new NotImplementedException() // Should warn
};

#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
