using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace App.Frontend.BackendLink;

// TODO: Figure out a better way to access the `wwwroot/dist/assets/` directory without giving the user access to the other files.
// Currently this only affects the `manifest.json` file, but I can foresee this being a problem for other files.

internal static class FullStackGlue
{
	
	public const string BuildOutputDirectory = "dist/";

	internal static Maybe<FrontendSources> GetSources(string entryFileName)
	{
		Log.Debug(messageTemplate: "Getting source from entryFileName {EntryFileName}", entryFileName);

		const string buildOutputDirectory = BuildOutputDirectory + "manifest.json";
		using var file = File.OpenText("wwwroot/" + buildOutputDirectory);
		using var reader = new JsonTextReader(file);
		
		var obj = (JObject)JToken.ReadFrom(reader);
		
		foreach (var (key, value) in obj)
		{
			if (key != $"src/entries/{entryFileName}.ts" || value is null)
			{
				continue;
			}

			var sources = JsonConvert.DeserializeObject<FrontendSources>(value.ToString());

			return sources;
		}

		return Maybe<FrontendSources>.None;
	}
}

internal readonly struct FrontendSources
{
	[JsonProperty(propertyName: "css")]
	public readonly IEnumerable<string> CssFiles;

	[JsonProperty(propertyName: "file")]
	public readonly string JavascriptFile;

	public void Deconstruct(out IEnumerable<string> css, out string js)
	{
		css = CssFiles;
		js = JavascriptFile;
	}
}