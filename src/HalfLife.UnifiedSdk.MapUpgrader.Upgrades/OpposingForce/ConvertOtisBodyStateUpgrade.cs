using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Converts <c>monster_otis</c> <c>bodystate</c> keyvalues to no longer include the <c>Random</c> value,
    /// which is equivalent to <c>Holstered</c>.
    /// </summary>
    internal sealed class ConvertOtisBodyStateUpgrade : MapUpgrade
    {
        private const string BodyStateKey = "bodystate";

        private enum BodyState
        {
            Random = -1,
            Holstered = 0,
            Drawn = 1
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var otis in context.Map.Entities.OfClass("monster_otis"))
            {
                if (!otis.ContainsKey(BodyStateKey))
                {
                    continue;
                }

                var bodystate = otis.GetInteger(BodyStateKey);

                if (bodystate == (int)BodyState.Random)
                {
                    bodystate = (int)BodyState.Holstered;
                }

                otis.SetInteger(BodyStateKey, bodystate);
            }
        }
    }
}
