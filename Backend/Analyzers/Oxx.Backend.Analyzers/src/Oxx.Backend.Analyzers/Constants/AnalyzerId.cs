namespace Oxx.Backend.Analyzers.Constants;

public static class AnalyzerId
{
	/// <summary>
	/// Prefix for all analyzer IDs provided by this project.
	/// </summary>
	private const string Prefix = "OXX";

	public const string RequiredProperty = Prefix + "0001";
	public const string OneOfSwitchExpressionMissingCases = Prefix + "9001";
	public const string OneOfSwitchExpressionImpossibleCases = Prefix + "9002";
}
