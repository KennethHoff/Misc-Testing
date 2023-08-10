// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using OneOf;
using OneOf.Types;

namespace Oxx.Backend.Analyzers.Sample;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
public class RequiredPropertyExamples
{
	// public string FirstName { get; set; }
	// public string LastName { get; set; }
	// public string Email { get; set; }

	public static void DoThing()
	{
		var result = new OneOf<NotFound, Success, False>();

		var message = result.Value switch
		{
			NotFound => "Not Found",
			Success => "Success!",
			False => throw new NotImplementedException()
		};
	}
}





#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
