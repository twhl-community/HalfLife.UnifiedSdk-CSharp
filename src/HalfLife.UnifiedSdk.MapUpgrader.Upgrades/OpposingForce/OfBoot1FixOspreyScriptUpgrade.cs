using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Prevents the Osprey in <c>ofboot1</c> from switching to the <c>rotor</c> animation
    /// and falling through the ground after loading a save game.
    /// </summary>
    internal sealed class OfBoot1FixOspreyScriptUpgrade : MapSpecificUpgrade
    {
        public OfBoot1FixOspreyScriptUpgrade()
            : base("ofboot1")
        {
        }
        protected override void ApplyCore(MapUpgradeContext context)
        {
            var script = context.Map.Entities
                .FirstOrDefault(e => e.ClassName == "scripted_sequence" && e.GetString("m_iszEntity") == "osprey");

            // Naming the script prevents it from immediately completing and removing the script itself.
            script?.SetTargetName("osprey_script");
        }
    }
}
