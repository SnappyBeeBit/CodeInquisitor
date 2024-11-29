using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SourceGeneration.Helpers;
using SourceGeneration.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
internal static class ConfigLoader
{
	public static void LoadSettings(this CodeInquisitor codeInquisitor, IncrementalGeneratorInitializationContext context)
	{
		var settingsProvider = context.AdditionalTextsProvider
				.Where(file => file.Path.EndsWith("code_inquisitor_settings.csv"))
				.Select((file, cancellationToken) => file.GetText(cancellationToken)?.ToString());

		IncrementalValuesProvider<Settings> settingsOutput = settingsProvider.Select((string settingsText, CancellationToken cancellationToken) =>
		{
			if (string.IsNullOrEmpty(settingsText))
				return null;
			List<string> lines = [.. settingsText.Split('\n')];

			List<string[]> CSV = new List<string[]>();
			foreach (string line in lines)
			{
				CSV.Add(line.Split(','));
			}
			Settings settings = new();
			foreach(string[] line in CSV)
			{
				if (line[0] == "TernaryDensity")
				{
					settings.TernaryDensity = Convert.ToInt32(line[1]);
				}
				else if (line[0] == "TernaryLineCount")
				{
					settings.TernaryLineCount = Convert.ToInt32(line[1]);
				}
				else if (line[0] == "ClassSize")
				{
					settings.ClassSize = Convert.ToInt32(line[1]);
				}
				else if (line[0] == "ReturnSize")
				{
					settings.ReturnSize = Convert.ToInt32(line[1]);
				}
			}
			return settings;
		});

		context.RegisterSourceOutput(settingsOutput, (spc, settings) =>
		{
			codeInquisitor.Settings = settings;
		});
	}
}

