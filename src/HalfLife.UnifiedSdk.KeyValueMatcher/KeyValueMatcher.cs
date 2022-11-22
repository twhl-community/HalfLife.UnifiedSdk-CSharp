using HalfLife.UnifiedSdk.Utilities.Entities;
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

        public KeyValuePair<string, string>? Match(Entity entity)
        {
            if (ClassNamePattern?.IsMatch(entity.ClassName) == false)
            {
                return null;
            }

            if (KeyPattern is null && ValuePattern is null)
            {
                return new("classname", entity.ClassName);
            }

            var result = entity.WithoutClassName().FirstOrDefault(kv => KeyPattern?.IsMatch(kv.Key) != false && ValuePattern?.IsMatch(kv.Value) != false);

            if (result.Key is null)
            {
                return null;
            }

            return result;
        }
    }
}
