using HalfLife.UnifiedSdk.Formats.Skill;
using HalfLife.UnifiedSdk.Utilities.Logging;
using Newtonsoft.Json;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace HalfLife.UnifiedSdk.Skill2Json
{
    internal static class Program
    {
        const string TargetExtension = ".json";

        public static async Task<int> Main(string[] args)
        {
            var inputFileName = new Argument<FileInfo>("filename", "skill.cfg to convert");

            var outputFileName = new Option<FileInfo?>("--output-filename",
                getDefaultValue: () => null,
                "If provided, the name of the file to write the skill.json contents to");

            var rootCommand = new RootCommand("Half-Life Unified SDK skill.cfg to skill.json converter")
            {
                inputFileName,
                outputFileName
            };

            rootCommand.SetHandler((fileName, outputFileName, logger) =>
                {
                    if (fileName.Extension == TargetExtension)
                    {
                        throw new ArgumentException($"Input file \"{fileName.FullName}\" has the same extension as the target file type", nameof(fileName));
                    }

                    //Default to input with different extension.
                    outputFileName ??= new FileInfo(Path.ChangeExtension(fileName.FullName, TargetExtension));

                    var data = SkillConverter.Convert(fileName.OpenRead());

                    data.Sections[0].Description = $"Converted skill.cfg values from {Path.Combine(fileName.Directory?.Name ?? string.Empty, fileName.Name)}";

                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    var json = JsonConvert.SerializeObject(data.Sections, settings);

                    logger.Information("Writing output to \"{FileName}\"", outputFileName.FullName);

                    using var writer = new StreamWriter(outputFileName.OpenWrite());

                    writer.Write(json);
                },
                inputFileName, outputFileName, LoggerBinder.Instance);

            return await rootCommand.InvokeAsync(args);
        }
    }
}