#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).





using OneOf;

namespace Oxx.Backend.Analyzers.Sample;

public class OneOfSwitchExpressionExamples
{
	public static void DoThing()
	{
		OneOf<string, bool, uint, sbyte, char> oneOf = true;

		var message = oneOf.Value switch
		{
			uint x => "hmm",
			char x => throw new NotImplementedException(),
			string x => throw new NotImplementedException(),
			bool x => throw new NotImplementedException(),
			sbyte x => throw new NotImplementedException()
		};
	}
}






#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
