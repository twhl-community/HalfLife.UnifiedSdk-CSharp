using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Renames certain animations referenced by <c>scripted_sequence</c>s targeting entities using <c>models/console_civ_scientist.mdl</c>.
    /// </summary>
    internal sealed class RenameConsoleCivAnimationsUpgrade : MapUpgrade
    {
        private static readonly ImmutableDictionary<string, string> AnimationRemap = new Dictionary<string, string>
        {
            { "idle1", "console_idle1" },
            { "work", "console_work" },
            { "shocked", "console_shocked" },
            { "sneeze", "console_sneeze" }
        }.ToImmutableDictionary();

        protected override void ApplyCore(MapUpgradeContext context)
        {
            ScriptedSequenceUtilities.RenameAnimations(context, null, "models/console_civ_scientist.mdl", AnimationRemap);
        }
    }
}
