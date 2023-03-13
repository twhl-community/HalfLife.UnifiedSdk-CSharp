using Newtonsoft.Json;
using Serilog;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Formats.Materials
{
    /// <summary>
    /// Converts original Half-Life <c>materials.txt</c> data to <c>materials.json</c>.
    /// </summary>
    public static class MaterialsConverter
    {
        private static readonly Regex LineRegex = new(@"^([A-Z])\s+(\w+)$");

        public static void Convert(Stream inputStream, Stream outputStream, ILogger logger)
        {
            using var reader = new StreamReader(inputStream);
            using var writer = new JsonTextWriter(new StreamWriter(outputStream));

            Dictionary<string, string> knownMaterials = new();

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartObject();

            int lineNumber = 0;

            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                ++lineNumber;

                // Extract any comments that might be present.

                int commentIndex = line.IndexOf("//");

                if (commentIndex != -1)
                {
                    // This can either be a comment on its own or a comment that follows a material entry.
                    // If it is a comment that followed an entry then it should be placed before the key in the JSON file.
                    writer.WriteWhitespace(Environment.NewLine);
                    writer.WriteWhitespace("\t");

                    writer.WriteComment(line[(commentIndex + 2)..]);

                    line = line[..commentIndex];
                }

                line = line.Trim();

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                // Special case: the original materials file has an incorrectly commented out entry that won't match the regex.
                if (line == "/D OUT_RK1")
                {
                    logger.Information("Ignoring line {LineNumber} (\"{Line}\"): known bad syntax, treating as comment",
                        lineNumber, line);
                    continue;
                }

                var match = LineRegex.Match(line);

                if (!match.Success)
                {
                    throw new ConverterException($"Invalid entry: {line}")
                    {
                        LineNumber = lineNumber
                    };
                }

                var materialName = match.Groups[2].Value;
                var type = match.Groups[1].Value.ToUpperInvariant();

                if (knownMaterials.TryGetValue(materialName, out var existingType))
                {
                    if (type == existingType)
                    {
                        logger.Warning("Ignoring duplicate material \"{Material}\"", materialName);
                    }
                    else
                    {
                        logger.Warning(
                            "Ignoring duplicate material \"{Material}\" with differing types (existing: {Existing}, new: {New})",
                            materialName, existingType, type);
                    }
                }
                else
                {
                    knownMaterials.Add(materialName, type);

                    writer.WritePropertyName(materialName);

                    writer.WriteStartObject();

                    writer.WritePropertyName("Type");
                    writer.WriteValue(type);

                    writer.WriteEndObject();
                }
            }

            writer.WriteEndObject();

            logger.Information("Wrote {Count} materials", knownMaterials.Count);
        }
    }
}
