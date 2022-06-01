using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Renames certain animations referenced by <c>scripted_sequence</c>s targeting <c>monster_otis</c>
    /// or entities using its model to use the new animation names.
    /// </summary>
    internal sealed class RenameOtisAnimationsUpgrade : IMapUpgradeAction
    {
        private const string ScriptedSequenceTargetKey = "m_iszEntity";
        private const string ScriptedSequencePlayKey = "m_iszPlay";

        private static readonly ImmutableDictionary<string, string> AnimationRemap = new Dictionary<string, string>
        {
            { "fence", "otis_fence" },
            { "wave", "otis_wave" }
        }.ToImmutableDictionary();

        public void Apply(MapUpgradeContext context)
        {
            foreach (var script in context.Map.Entities
                .OfClass("scripted_sequence")
                .Where(e => e.ContainsKey(ScriptedSequenceTargetKey)))
            {
                var target = context.Map.Entities
                    .WhereTargetName(script.GetString(ScriptedSequenceTargetKey))
                    .FirstOrDefault();

                if (target is null)
                {
                    continue;
                }

                if (target.ClassName != "monster_otis" && target.GetModel() != "models/otis.mdl")
                {
                    continue;
                }

                if (script.GetStringOrNull(ScriptedSequencePlayKey) is { } playAnimation
                    && AnimationRemap.TryGetValue(playAnimation, out var replacement))
                {
                    script.SetString(ScriptedSequencePlayKey, replacement);
                }
            }
        }
    }
}
