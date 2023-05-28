using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.HalfLife
{
    /// <summary>
    /// Fixes Nihilanth's dialogue not playing at the start of <c>c4a2</c> (Gonarch's Lair).
    /// </summary>
    internal sealed class C4a2FixNihilanthDialogueUpgrade : MapSpecificUpgrade
    {
        public C4a2FixNihilanthDialogueUpgrade()
            : base("c4a2")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            // The sound is set to play on map start; this turns it off.
            context.Map.Entities.FirstOrDefault(e => e.GetTarget() == "c4a2_startaudio")?.SetInteger("triggerstate", 1);
        }
    }
}
