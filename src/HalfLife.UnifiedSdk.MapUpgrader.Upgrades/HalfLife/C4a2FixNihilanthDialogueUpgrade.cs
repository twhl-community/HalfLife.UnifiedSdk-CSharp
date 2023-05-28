using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.HalfLife
{
    /// <summary>
    /// Fixes Nihilanth's dialogue not playing at the start of <c>c4a2</c> (Gonarch's Lair).
    /// </summary>
    internal sealed class C4a2FixNihilanthDialogueUpgrade : MapSpecificUpgradeAction
    {
        public C4a2FixNihilanthDialogueUpgrade()
            : base("c4a2")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            // The sound is set to play on map start; this turns it off.
            var triggerAuto = context.Map.Entities.FirstOrDefault(e => e.GetTarget() == "c4a2_startaudio");

            if (triggerAuto is not null)
            {
                triggerAuto.SetInteger("triggerstate", 1);
                //context.Map.Entities.Remove(triggerAuto);
            }
        }
    }
}
