using HalfLife.UnifiedSdk.Utilities.Tools;
using Newtonsoft.Json;
using System.Globalization;

namespace HalfLife.UnifiedSdk.Formats.Skill
{
    /// <summary>
    /// Converts original Half-Life <c>skill.cfg</c> data to <c>skill.json</c>.
    /// </summary>
    public static class SkillConverter
    {
        public static void Convert(Stream inputStream, Stream outputStream, string description)
        {
            using var reader = new StreamReader(inputStream, leaveOpen: true);

            HashSet<string> knownVariables = new();

            using var writer = new JsonTextWriter(new StreamWriter(outputStream, leaveOpen: true));

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteComment(description);

            writer.WriteWhitespace(Environment.NewLine);

            writer.WriteStartObject();

            int lineNumber = 0;

            string? line;

            string comments = string.Empty;

            while ((line = reader.ReadLine()) != null)
            {
                ++lineNumber;

                var tokenizer = new Tokenizer(line, allowComments: true);

                //Each line should be two tokens. Comments can occur on their own line or after the tokens.
                while (tokenizer.Next() && tokenizer.Type == TokenType.Comment)
                {
                    comments = StringUtilities.ConcatComments(comments, tokenizer.Token);
                }

                if (tokenizer.Type != TokenType.Text && !tokenizer.Next())
                {
                    //Empty or comments. Keep comments for next valid token.
                    continue;
                }

                var key = tokenizer.Token.ToString();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ConverterException("Skill variable has empty name")
                    {
                        LineNumber = lineNumber
                    };
                }

                if (!tokenizer.Next())
                {
                    throw new ConverterException($"Skill variable \"{key}\" missing value")
                    {
                        LineNumber = lineNumber
                    };
                }

                if (tokenizer.Type != TokenType.Text)
                {
                    throw new ConverterException($"Skill variable \"{key}\" missing value (commented out?)")
                    {
                        LineNumber = lineNumber
                    };
                }

                if (!float.TryParse(tokenizer.Token, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                {
                    throw new ConverterException($"Skill variable \"{key}\" has invalid value \"{tokenizer.Token.ToString()}\"")
                    {
                        LineNumber = lineNumber
                    };
                }

                if (knownVariables.Contains(key))
                {
                    throw new ConverterException($"Duplicate skill variable \"{key}\"")
                    {
                        LineNumber = lineNumber
                    };
                }

                if (!string.IsNullOrEmpty(comments))
                {
                    // Force a newline before the comment. This will still place it before the comma.
                    if (knownVariables.Count > 0)
                    {
                        writer.WriteWhitespace(Environment.NewLine);
                        writer.WriteWhitespace("\t");
                    }

                    writer.WriteComment(StringUtilities.IndentLines(comments, 1, '\t', false));
                    comments = string.Empty;
                }

                knownVariables.Add(key);

                bool isInteger = Math.Abs(value % 1) <= (double.Epsilon * 100);

                writer.WritePropertyName(key);
                writer.WriteRawValue(value.ToString(isInteger ? "F0" : "F2", CultureInfo.InvariantCulture));

                while (tokenizer.Next() && tokenizer.Type == TokenType.Comment)
                {
                    comments = StringUtilities.ConcatComments(comments, tokenizer.Token);
                }

                if (!string.IsNullOrEmpty(comments))
                {
                    writer.WriteComment(comments);
                    comments = string.Empty;
                }

                if (tokenizer.Type != TokenType.None)
                {
                    throw new ConverterException("Too many tokens on line")
                    {
                        LineNumber = lineNumber
                    };
                }
            }

            // Write any remaining comments.
            if (!string.IsNullOrEmpty(comments))
            {
                writer.WriteComment(comments);
                comments = string.Empty;
            }

            writer.WriteEndObject();
        }
    }
}
