using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.KeyValueMatcher
{
    /// <summary>
    /// Matches classnames, keys and values in an entity.
    /// </summary>
    internal sealed class KeyValueMatcher
    {
        public Regex? ClassNamePattern { get; set; }

        public Regex? KeyPattern { get; set; }

        public Regex? ValuePattern { get; set; }

        public string? FlagsToMatch { get; set; }

        public FlagsMatchMode FlagsMatchMode { get; set; } = FlagsMatchMode.Inclusive;

        public int Flags { get; set; }

        public KeyValuePair<string, string>? Match(Entity entity)
        {
            if (ClassNamePattern?.IsMatch(entity.ClassName) == false)
            {
                return null;
            }

            if (KeyPattern is null && ValuePattern is null && FlagsToMatch is null)
            {
                return new(KeyValueUtilities.ClassName, entity.ClassName);
            }

            KeyValuePair<string, string> result = new();

            if (KeyPattern is not null || ValuePattern is not null)
            {
                result = entity.WithoutClassName()
                    .FirstOrDefault(kv => KeyPattern?.IsMatch(kv.Key) != false && ValuePattern?.IsMatch(kv.Value) != false);

                if (result.Key is null)
                {
                    return null;
                }
            }

            // Additionally filter by flags if provided.
            if (FlagsToMatch is not null)
            {
                var flags = entity.GetInteger(FlagsToMatch);

                bool containsFlag = (flags & Flags) != 0;

                if (FlagsMatchMode == FlagsMatchMode.Inclusive)
                {
                    if (!containsFlag)
                    {
                        return null;
                    }
                }
                else
                {
                    if (containsFlag)
                    {
                        return null;
                    }
                }

                // No key to match against, so use flags instead.
                if (result.Key is null)
                {
                    result = new(FlagsToMatch, flags.ToString());
                }
            }

            return result;
        }
    }
}
