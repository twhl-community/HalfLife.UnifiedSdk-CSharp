using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Fixes <c>ambient_generic</c> entities using non-looping sounds
    /// to stop them from restarting when loading a save game.
    /// </summary>
    internal sealed class FixNonLoopingSoundsUpgrade : MapUpgrade
    {
        private const string MessageKey = "message";

        private static readonly ImmutableHashSet<string> MapSoundFileNames = ImmutableHashSet.Create(
            "ambience/alienflyby1.wav",
            "misc/ear_ringing.wav",
            "weapons/explode3.wav",
            "weapons/explode4.wav",
            "weapons/mortar.wav",
            "nihilanth/nil_alone.wav",
            "nihilanth/nil_win.wav",
            "Gonarch/gon_die1.wav");

        private enum AmbientGenericSpawnFlags
        {
            NotToggled = 1 << 5
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities.OfClass("ambient_generic"))
            {
                var soundFileName = entity.GetString(MessageKey);

                if (!MapSoundFileNames.Contains(soundFileName))
                {
                    continue;
                }

                int flags = entity.GetSpawnFlags();

                if ((flags & (int)AmbientGenericSpawnFlags.NotToggled) == 0)
                {
                    flags |= (int)AmbientGenericSpawnFlags.NotToggled;

                    entity.SetSpawnFlags(flags);
                }
            }
        }
    }
}
