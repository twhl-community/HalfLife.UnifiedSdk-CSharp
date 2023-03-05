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
        private static readonly Regex LineRegex = new(@"^\s*([A-Z])?(\s+\w+)?\s*(\/\/.*)?$");

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

                var match = LineRegex.Match(line);

                if (!match.Success)
                {
                    continue;
                }

                // This can either be a comment on its own or a comment that follows a material entry.
                // If it is a comment that followed an entry then it should be placed before the key in the JSON file.
                if (match.Groups[3].Success)
                {
                    writer.WriteWhitespace(Environment.NewLine);
                    writer.WriteWhitespace("\t");

                    writer.WriteComment(match.Groups[3].Value[2..]);
                }

                if (match.Groups[1].Success && match.Groups[2].Success)
                {
                    var materialName = match.Groups[2].Value.Trim();
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
                else if(match.Groups[1].Success || match.Groups[2].Success)
                {
                    throw new ConverterException("Invalid material entry")
                    {
                        LineNumber = lineNumber
                    };
                }
            }

            writer.WriteEndObject();

            logger.Information("Wrote {Count} materials", knownMaterials.Count);
        }
    }
}
