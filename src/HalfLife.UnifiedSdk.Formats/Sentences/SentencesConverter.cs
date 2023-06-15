using HalfLife.UnifiedSdk.Utilities.Tools;
using Newtonsoft.Json;
using Serilog;
using System.Text;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Formats.Sentences
{
    /// <summary>
    /// Converts original Half-Life <c>sentences.txt</c> data to <c>sentences.json</c>.
    /// </summary>
    public static class SentencesConverter
    {
        private static readonly Regex LineRegex = new(@"^(?<name>\w\S+)\s+(?<contents>.+)$");

        private sealed record SentenceGroup(string Name, List<string> SentenceNames)
        {
            public override string ToString()
            {
                // Don't include the struct name.
                StringBuilder builder = new();
                PrintMembers(builder);
                return builder.ToString();
            }
        }

        public static void Convert(Stream inputStream, Stream outputStream, ILogger logger)
        {
            using var reader = new StreamReader(inputStream);
            using var writer = new JsonTextWriter(new StreamWriter(outputStream));

            Dictionary<string, string> sentences = new();
            List<SentenceGroup> groups = new(); // Preserve order of groups in file when writing to disk.

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartObject();

            writer.WritePropertyName("Sentences");
            writer.WriteStartArray();

            int lineNumber = 0;

            int parsedSentencesCount = 0;

            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                ++lineNumber;

                Tokenizer tokenizer = new(line, allowComments: true);

                ParseComment();

                line = line.Trim();

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var match = LineRegex.Match(line);

                if (!match.Success)
                {
                    throw new ConverterException("Sentence entry incorrect")
                    {
                        LineNumber = lineNumber
                    };
                }

                ++parsedSentencesCount;

                ParseLine(match);
            }

            writer.WriteEndArray();

            if (groups.Count > 0)
            {
                writer.WritePropertyName("Groups");
                writer.WriteStartObject();

                foreach (var group in groups)
                {
                    if (group.Name.Length == 1)
                    {
                        writer.WriteWhitespace(Environment.NewLine);
                        writer.WriteWhitespace("\t\t");
                        writer.WriteComment("Note: this group was ignored by the game's original parser");
                    }

                    writer.WritePropertyName(group.Name);
                    writer.WriteStartArray();

                    foreach (var sentence in group.SentenceNames)
                    {
                        writer.WriteValue(sentence);
                    }

                    writer.WriteEndArray();
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();

            logger.Information("Wrote {SentenceCount} sentences and {GroupCount} groups", parsedSentencesCount, groups.Count);

            void ParseComment()
            {
                // Extract any comments that might be present.
                int commentIndex = line.IndexOf("//");

                if (commentIndex != -1)
                {
                    // This can either be a comment on its own or a comment that follows a sentence entry.
                    // If it is a comment that followed an entry then it should be placed before the key in the JSON file.
                    writer.WriteWhitespace(Environment.NewLine);
                    writer.WriteWhitespace("\t");

                    writer.WriteComment(line[(commentIndex + 2)..]);

                    line = line[..commentIndex];
                }
            }

            void ParseLine(Match match)
            {
                var sentenceName = match.Groups["name"].Value;
                var contents = match.Groups["contents"].Value;

                if (sentences.TryGetValue(sentenceName, out var existingSentence))
                {
                    if (contents == existingSentence)
                    {
                        logger.Warning("Duplicate sentence \"{Name}\"", sentenceName);
                    }
                    else
                    {
                        logger.Warning("Duplicate sentence \"{Name}\" with differing words:", sentenceName);
                        logger.Warning("Existing: {Existing}", existingSentence);
                        logger.Warning("New:      {New}", contents);
                    }
                }
                else
                {
                    sentences.Add(sentenceName, contents);
                }

                writer.WriteValue($"{sentenceName} {contents}");

                int lastNonNumeric = -1;

                for (int i = sentenceName.Length - 1; i >= 0; --i)
                {
                    if (!char.IsDigit(sentenceName[i]))
                    {
                        lastNonNumeric = i;
                        break;
                    }
                }

                if (lastNonNumeric == -1 || lastNonNumeric == (sentenceName.Length - 1))
                {
                    // No digit at end or nothing but digits.
                    return;
                }

                // This is part of a group.
                var groupName = sentenceName[0..(lastNonNumeric + 1)];

                var group = groups.Find(g => g.Name == groupName);

                if (group is null)
                {
                    group = new(groupName, new());
                    groups.Add(group);
                }

                if (group.SentenceNames.Contains(sentenceName))
                {
                    logger.Warning("Ignoring duplicate sentence {Name} in group", sentenceName);
                    return;
                }

                group.SentenceNames.Add(sentenceName);

                int expectedIndex = int.Parse(sentenceName.AsSpan()[(lastNonNumeric + 1)..]);
                int actualIndex = group.SentenceNames.Count - 1;

                if (expectedIndex != actualIndex)
                {
                    logger.Warning("Sentence {Name} has incorrect group index {ActualIndex}; expected {ExpectedIndex}",
                        sentenceName, actualIndex, expectedIndex);
                }
            }
        }
    }
}
