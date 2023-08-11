#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

using OneOf;

namespace Oxx.Backend.Analyzers.Sample;

public class OneOfSwitchExpressionExamples
{
	public static void DoThing()
	{
		OneOf<IReadOnlyDictionary<int, (bool, bool, bool, string)>, bool, uint, sbyte, char, IReadOnlyList<string>, IReadOnlyList<IReadOnlyList<string>>> oneOf = true;

		var message = oneOf.Value switch
		{
			uint x => "hmm",
			bool x => throw new NotImplementedException(),
			sbyte x => throw new NotImplementedException(),
			IReadOnlyList<string> x => throw new NotImplementedException(),
			IReadOnlyList<IReadOnlyList<string>> x => throw new NotImplementedException(),
			IReadOnlyDictionary<int, (bool, bool, bool, string)> x => throw new NotImplementedException(),
			char x => throw new NotImplementedException(),
			IReadOnlyList<string> x => throw new NotImplementedException(),
			IReadOnlyList<IReadOnlyList<string>> x => throw new NotImplementedException()
		};
	}
}






#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
