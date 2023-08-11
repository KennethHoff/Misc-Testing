using OneOf;
using OneOf.Types;

namespace Oxx.Backend.Analyzers.Sample;

public class OneOfSwitchExpressionExamples
{
	public static void DoThing()
	{
		OneOf<string, bool, OneOfSwitchExpressionExamples> oneOf = true;

		string message = oneOf.Value switch
		{
			String value => throw new NotImplementedException(),
			Boolean value => throw new NotImplementedException(),
			OneOfSwitchExpressionExamples value => throw new NotImplementedException()
		};

		Console.WriteLine();
	}
}
