using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>Fixes sounds on <c>ba_xen3</c> to stop them from restarting when loading a save game.</summary>
    internal sealed class BaXen3FixFlyBySoundsUpgrade : MapSpecificUpgradeAction
    {
        private const string MessageKey = "message";
        private const string SoundFileName = "ambience/alienflyby1.wav";

        private enum AmbientGenericSpawnFlags
        {
            NotToggled = 1 << 5
        }

        public BaXen3FixFlyBySoundsUpgrade()
            : base("ba_xen3")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities.OfClass("ambient_generic"))
            {
                if (entity.HasKeyValue(MessageKey, SoundFileName))
                {
                    int flags = entity.GetSpawnFlags();

                    flags |= (int)AmbientGenericSpawnFlags.NotToggled;

                    entity.SetSpawnFlags(flags);
                }
            }
        }
    }
}
