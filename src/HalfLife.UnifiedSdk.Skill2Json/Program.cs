using HalfLife.UnifiedSdk.Formats.Skill;
using Newtonsoft.Json;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;

const string TargetExtension = ".json";

var inputFileName = new Argument<FileInfo>("filename", "skill.cfg to convert");

var outputFileName = new Option<FileInfo>("--output-filename", "If provided, the name of the file to write the skill.json contents to");

var rootCommand = new RootCommand
{
    inputFileName,
    outputFileName
};

rootCommand.Description = "Half-Life Unified SDK skill.cfg to skill.json converter";

rootCommand.SetHandler((FileInfo fileName, FileInfo? outputFileName, IConsole console) =>
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

        console.Out.WriteLine($"Writing output to \"{outputFileName.FullName}\"");

        using var writer = new StreamWriter(outputFileName.OpenWrite());

        writer.Write(json);
    },
    inputFileName, outputFileName);

return await rootCommand.InvokeAsync(args);
