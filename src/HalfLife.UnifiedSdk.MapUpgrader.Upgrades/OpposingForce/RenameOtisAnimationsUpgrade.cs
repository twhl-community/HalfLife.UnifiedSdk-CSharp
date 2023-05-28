using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Renames certain animations referenced by <c>scripted_sequence</c>s targeting <c>monster_otis</c>
    /// or entities using its model to use the new animation names.
    /// </summary>
    internal sealed class RenameOtisAnimationsUpgrade : MapUpgrade
    {
        private static readonly ImmutableDictionary<string, string> AnimationRemap = new Dictionary<string, string>
        {
            { "fence", "otis_fence" },
            { "wave", "otis_wave" }
        }.ToImmutableDictionary();

        protected override void ApplyCore(MapUpgradeContext context)
        {
            ScriptedSequenceUtilities.RenameAnimations(context, "monster_otis", "models/otis.mdl", AnimationRemap);
        }
    }
}
