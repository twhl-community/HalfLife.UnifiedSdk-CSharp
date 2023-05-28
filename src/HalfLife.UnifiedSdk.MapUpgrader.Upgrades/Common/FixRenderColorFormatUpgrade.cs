using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Fixes the use of invalid render color formats in some maps.
    /// </summary>
    internal sealed class FixRenderColorFormatUpgrade : MapUpgrade
    {
        private static readonly Regex InvalidFormatRegex = new(@"^(\d+), (\d+), (\d+)$");

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities)
            {
                if (entity.TryGetValue("rendercolor", out var value))
                {
                    var match = InvalidFormatRegex.Match(value);

                    if (match.Success)
                    {
                        var correctColor = $"{match.Groups[1].Value} {match.Groups[2].Value} {match.Groups[3].Value}";

                        entity.SetString("rendercolor", correctColor);
                    }
                }
            }
        }
    }
}
