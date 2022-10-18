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
            if (ClassNamePattern is not null && !ClassNamePattern.IsMatch(entity.ClassName))
            {
                return false;
            }

            if (KeyPattern is null && ValuePattern is null)
            {
                return true;
            }

            foreach (var keyValue in entity.WithoutClassName())
            {
                if (KeyPattern is not null && !KeyPattern.IsMatch(keyValue.Key))
                {
                    continue;
                }

                if (ValuePattern is not null && !ValuePattern.IsMatch(keyValue.Value))
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
