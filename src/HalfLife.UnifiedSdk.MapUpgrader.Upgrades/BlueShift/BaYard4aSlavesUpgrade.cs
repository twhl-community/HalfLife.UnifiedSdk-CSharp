using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Fixes the Alien Slaves in <c>ba_yard4a</c> being resurrected by triggering them
    /// instead of the <c>scripted_sequence</c> keeping them in stasis.
    /// </summary>
    internal sealed class BaYard4aSlavesUpgrade : MapSpecificUpgrade
    {
        public BaYard4aSlavesUpgrade()
            : base("ba_yard4a")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            ModifyScript(context, "frozen_slave_1");
            ModifyScript(context, "frozen_slave_2");
        }

        private static void ModifyScript(MapUpgradeContext context, string slaveName)
        {
            var breakable = context.Map.Entities.FirstOrDefault(e => e.ClassName == "func_breakable" && e.GetTarget() == slaveName);
            var script = context.Map.Entities.FirstOrDefault(e =>
                e.ClassName == ScriptedSequenceUtilities.ClassName
                && e.GetString(ScriptedSequenceUtilities.TargetKey) == slaveName);

            if (breakable is null || script is null)
            {
                return;
            }

            var scriptName = slaveName + "_script";

            script.SetTargetName(scriptName);
            breakable.SetTarget(scriptName);
        }
    }
}
