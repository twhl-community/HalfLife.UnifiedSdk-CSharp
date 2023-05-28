using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Numerics;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Converts the obsolete <c>angle</c> keyvalue to <c>angles</c>.
    /// This is normally done by the engine, but to avoid having to account for both keyvalues in other upgrades this is done here.
    /// </summary>
    internal sealed class ConvertAngleToAnglesUpgrade : MapUpgrade
    {
        private const string AngleKey = "angle";

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities.Where(e => e.ContainsKey(AngleKey)))
            {
                var angle = (float)entity.GetDouble(AngleKey);

                if (angle >= 0)
                {
                    var angles = entity.GetAngles();

                    angles.Y = angle;

                    entity.SetAngles(angles);
                }
                else
                {
                    var angles = Vector3.Zero;

                    if (MathF.Floor(angle) == -1)
                    {
                        angles.X = -90;
                    }
                    else
                    {
                        angles.X = 90;
                    }

                    entity.SetAngles(angles);
                }

                entity.Remove(AngleKey);
            }
        }
    }
}
