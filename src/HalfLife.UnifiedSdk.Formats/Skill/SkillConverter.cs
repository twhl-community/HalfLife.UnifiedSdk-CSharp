using HalfLife.UnifiedSdk.Utilities.Tools;
using System.Globalization;

namespace HalfLife.UnifiedSdk.Formats.Skill
{
    /// <summary>
    /// Converts original Half-Life <c>skill.cfg</c> data to <c>skill.json</c>.
    /// </summary>
    public static class SkillConverter
    {
        public static SkillData Convert(Stream stream)
        {
            using var reader = new StreamReader(stream);

            var data = new SkillData();

            var section = new SkillSection();

            data.Sections.Add(section);

            int lineNumber = 0;

            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                ++lineNumber;

                var tokenizer = new Tokenizer(line);

                //Each line should be two tokens.
                if (!tokenizer.Next())
                {
                    //Empty or comment.
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

                if (!float.TryParse(tokenizer.Token, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                {
                    throw new ConverterException($"Skill variable \"{key}\" has invalid value \"{tokenizer.Token.ToString()}\"")
                    {
                        LineNumber = lineNumber
                    };
                }

                if (!section.Variables.TryAdd(key, value))
                {
                    throw new ConverterException($"Duplicate skill variable \"{key}\"")
                    {
                        LineNumber = lineNumber
                    };
                }

                if (tokenizer.Next())
                {
                    throw new ConverterException("Too many tokens on line")
                    {
                        LineNumber = lineNumber
                    };
                }
            }

            return data;
        }
    }
}
