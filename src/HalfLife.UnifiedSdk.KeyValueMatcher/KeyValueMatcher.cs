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

        public bool IsMatch(Entity entity)
        {
            if (ClassNamePattern?.IsMatch(entity.ClassName) == false)
            {
                return false;
            }

            if (KeyPattern is null && ValuePattern is null)
            {
                return true;
            }

            return entity.WithoutClassName().Any(kv => KeyPattern?.IsMatch(kv.Key) != false && ValuePattern?.IsMatch(kv.Value) != false);
        }
    }
}
