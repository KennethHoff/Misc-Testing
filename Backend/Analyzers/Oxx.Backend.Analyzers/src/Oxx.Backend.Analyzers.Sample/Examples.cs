// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace Oxx.Backend.Analyzers.Sample;

public class RequiredPropertyExamples
{
	public string FirstName { get; set; }
	public required string LastName { get; set; }
	public string Email { get; set; }

	public RequiredPropertyExamples()
	{
		FirstName = "John";
	}
}
