using HalfLife.UnifiedSdk.Formats.Sentences;
using HalfLife.UnifiedSdk.Utilities.Logging;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace HalfLife.UnifiedSdk.Sentences2Json
{
    internal static class Program
    {
        const string TargetExtension = ".json";

        public static async Task<int> Main(string[] args)
        {
            var inputFileName = new Argument<FileInfo>("filename", "Sentences definition to convert");

            var outputFileName = new Option<FileInfo?>("--output-filename",
                getDefaultValue: () => null,
                "If provided, the name of the file to write the sentences.json contents to.\n" +
                "Otherwise the file is saved to the source directory with the same name and 'json' extension.");

            var rootCommand = new RootCommand("Half-Life Unified SDK sentences.txt to sentences.json converter")
            {
                inputFileName,
                outputFileName
            };

            rootCommand.SetHandler((fileName, outputFileName, logger) =>
            {
                if (fileName.Extension == TargetExtension)
                {
                    throw new ArgumentException(
                        $"Input file \"{fileName.FullName}\" has the same extension as the target file type", nameof(fileName));
                }

                //Default to input with different extension.
                outputFileName ??= new FileInfo(Path.ChangeExtension(fileName.FullName, TargetExtension));

                logger.Information("Writing output to \"{FileName}\"", outputFileName.FullName);

                using var inputStream = fileName.OpenRead();
                using var outputStream = outputFileName.Open(FileMode.Create);

                SentencesConverter.Convert(inputStream, outputStream, logger);
            },
                inputFileName, outputFileName, LoggerBinder.Instance);

            return await rootCommand.InvokeAsync(args);
        }
    }
}