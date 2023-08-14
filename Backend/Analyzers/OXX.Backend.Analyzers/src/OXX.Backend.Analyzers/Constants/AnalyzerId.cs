namespace OXX.Backend.Analyzers.Constants;

public static class AnalyzerId
{
	/// <summary>
	/// Prefix for all analyzer IDs provided by this project.
	/// </summary>
	private const string Prefix = "OXX";

	/// <summary>
	/// If the analyzer reaches something thought to be impossible
	/// </summary>
	public const string Unreachable = Prefix + "0000";


	public static class General
	{
		public const string RequiredProperty = Prefix + "0001";
	}

	public static class OneOf
	{
		public const string SwitchExpressionMissingCases = Prefix + "9001";
		public const string SwitchExpressionImpossibleCases = Prefix + "9002";
		public const string SwitchExpressionDiscardPattern = Prefix + "9003";
		public const string ConvertMatchToSwitchExpression = Prefix + "9004"; // To be implemented
		public const string ConvertExpressionThrowingMethodToOneOfReturningMethod = Prefix + "9005"; // To be implemented
	}

    public static class BuiltIn
    {
        public const string NonExhaustiveSwitchExpression = "CS8509";
    }
}
