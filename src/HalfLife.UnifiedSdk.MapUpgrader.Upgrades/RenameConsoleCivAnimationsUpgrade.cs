using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    /// <summary>
    /// Renames certain animations referenced by <c>scripted_sequence</c>s targeting entities using <c>models/console_civ_scientist.mdl</c>.
    /// </summary>
    internal sealed class RenameConsoleCivAnimationsUpgrade : IMapUpgradeAction
    {
        private const string ScriptedSequenceTargetKey = "m_iszEntity";
        private const string ScriptedSequenceIdleKey = "m_iszIdle";
        private const string ScriptedSequencePlayKey = "m_iszPlay";

        private static readonly ImmutableList<string> KeysToCheck = ImmutableList.Create(ScriptedSequenceIdleKey, ScriptedSequencePlayKey);

        private static readonly ImmutableDictionary<string, string> AnimationRemap = new Dictionary<string, string>
        {
            { "idle1", "console_idle1" },
            { "work", "console_work" },
            { "shocked", "console_shocked" },
            { "sneeze", "console_sneeze" }
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

                if (target.GetModel() != "models/console_civ_scientist.mdl")
                {
                    continue;
                }

                foreach (var key in KeysToCheck)
                {
                    if (script.GetStringOrNull(key) is { } animation
                        && AnimationRemap.TryGetValue(animation, out var replacement))
                    {
                        script.SetString(key, replacement);
                    }
                }
            }
        }
    }
}
