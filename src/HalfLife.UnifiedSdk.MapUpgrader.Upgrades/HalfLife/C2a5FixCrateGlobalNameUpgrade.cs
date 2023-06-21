using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.HalfLife
{
    /// <summary>
    /// Removes the <c>globalname</c> keyvalue from the <c>func_breakable</c> crates next to the dam in <c>c2a5</c>.
    /// The globalname is left over from copy pasting the entity from the crates in the tunnel earlier in the map
    /// and causes these crates to disappear.
    /// </summary>
    internal sealed class C2a5FixCrateGlobalNameUpgrade : MapSpecificUpgrade
    {
        public C2a5FixCrateGlobalNameUpgrade()
            : base("c2a5")
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var breakable in context.Map.Entities
                .Where(e => e.ClassName == "func_breakable"
                && e.GetTargetName() == "artillery_deploy3_expl"
                && e.GetGlobalName() == "c2a4g_crate4"))
            {
                breakable.Remove(KeyValueUtilities.GlobalName);
            }
        }
    }
}
