using HalfLife.UnifiedSdk.Utilities.Tools;
using Newtonsoft.Json;
using Serilog;
using System.Text;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Formats.Hud
{
    /// <summary>
    /// Converts original Half-Life <c>hud.txt</c> data to <c>hud.json</c>.
    /// </summary>
    public static class HudConverter
    {
        /// <summary>
        /// Only this resolution is used by the Unified SDK, so any others need to be ignored.
        /// </summary>
        private const int SupportedResolution = 640;

        private static readonly Regex LineRegex = new(
            @"^(?<hudname>[\w]+)\s+(?<res>\d+)\s+(?<spritename>[\w]+)\s+(?<left>\d+)\s+(?<top>\d+)\s+(?<width>\d+)\s+(?<height>\d+)$");

        private sealed record HudSprite(string SpriteName, int Left, int Top, int Width, int Height)
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

            Dictionary<string, HudSprite> knownSprites = new();

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartObject();

            int lineNumber = 0;

            int parsedSpritesCount = 0;

            int? spriteCount = null;

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

                if (spriteCount is null)
                {
                    spriteCount = ParseSpriteCount();
                    logger.Information("{Count} sprites total specified", spriteCount);
                    continue;
                }

                var match = LineRegex.Match(line);

                if (!match.Success)
                {
                    throw new ConverterException("Sprite entry incorrect")
                    {
                        LineNumber = lineNumber
                    };
                }

                ++parsedSpritesCount;

                ParseLine(match);
            }

            writer.WriteEndObject();

            if (spriteCount is null)
            {
                throw new ConverterException("Missing sprite count")
                {
                    LineNumber = lineNumber
                };
            }

            logger.Information("Parsed {Count} hud sprites total", parsedSpritesCount);

            if (parsedSpritesCount > spriteCount)
            {
                logger.Warning("Parsed {Count} hud sprites more than specified ({OriginalCount})", parsedSpritesCount - spriteCount, spriteCount);
            }
            else if (parsedSpritesCount < spriteCount)
            {
                logger.Warning("Parsed {Count} hud sprites fewer than specified ({OriginalCount})", spriteCount - parsedSpritesCount, spriteCount);
            }

            logger.Information("Wrote {Count} hud sprites", knownSprites.Count);

            void ParseComment()
            {
                // Extract any comments that might be present.
                int commentIndex = line.IndexOf("//");

                if (commentIndex != -1)
                {
                    // This can either be a comment on its own or a comment that follows a sprite entry.
                    // If it is a comment that followed an entry then it should be placed before the key in the JSON file.
                    writer.WriteWhitespace(Environment.NewLine);
                    writer.WriteWhitespace("\t");

                    writer.WriteComment(line[(commentIndex + 2)..]);

                    line = line[..commentIndex];
                }
            }

            int ParseSpriteCount()
            {
                // First valid token must be a number.
                if (!int.TryParse(line, out var count))
                {
                    throw new ConverterException("Expected sprite count token")
                    {
                        LineNumber = lineNumber
                    };
                }

                if (count < 0)
                {
                    throw new ConverterException("Sprite count must be >= 0")
                    {
                        LineNumber = lineNumber
                    };
                }

                return count;
            }

            void ParseLine(Match match)
            {
                try
                {
                    var hudSpriteName = match.Groups["hudname"].Value;
                    int resolution = int.Parse(match.Groups["res"].Value);

                    if (resolution != SupportedResolution)
                    {
                        logger.Information("Ignoring sprite \"{Name}\" ({Index}) because it uses an unsupported resolution ({Resolution})",
                            hudSpriteName, parsedSpritesCount, resolution);
                        return;
                    }

                    int left = int.Parse(match.Groups["left"].Value);
                    int top = int.Parse(match.Groups["top"].Value);
                    int width = int.Parse(match.Groups["width"].Value);
                    int height = int.Parse(match.Groups["height"].Value);

                    HudSprite hudSprite = new(match.Groups["spritename"].Value, left, top, width, height);

                    if (knownSprites.TryGetValue(hudSpriteName, out var existingSprite))
                    {
                        if (hudSprite == existingSprite)
                        {
                            logger.Warning("Ignoring duplicate sprite \"{Sprite}\"", hudSpriteName);
                        }
                        else
                        {
                            logger.Warning(
                                "Ignoring duplicate sprite \"{Sprite}\" with differing sprite names and/or rectangles:",
                                hudSpriteName);

                            logger.Warning("Existing: {Existing}", existingSprite);
                            logger.Warning("New:      {New}", hudSprite);
                        }

                        return;
                    }

                    knownSprites.Add(hudSpriteName, hudSprite);

                    writer.WritePropertyName(hudSpriteName);

                    writer.WriteStartObject();

                    writer.WritePropertyName("SpriteName");
                    writer.WriteValue(hudSprite.SpriteName);

                    writer.WritePropertyName("Left");
                    writer.WriteValue(hudSprite.Left);

                    writer.WritePropertyName("Top");
                    writer.WriteValue(hudSprite.Top);

                    writer.WritePropertyName("Width");
                    writer.WriteValue(hudSprite.Width);

                    writer.WritePropertyName("Height");
                    writer.WriteValue(hudSprite.Height);

                    writer.WriteEndObject();
                }
                catch (Exception e) when (e is FormatException || e is OverflowException)
                {
                    throw new ConverterException("A number has an invalid value", e)
                    {
                        LineNumber = lineNumber
                    };
                }
            }
        }
    }
}
