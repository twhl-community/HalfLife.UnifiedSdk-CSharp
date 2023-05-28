using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Numerics;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Opposing Force and Blue Shift's shotgun world model has a different alignment.
    /// Since we're using the vanilla Half-Life model the angles need adjusting.
    /// This adjusts the angles to match the original model.
    /// </summary>
    internal sealed class AdjustShotgunAnglesUpgrade : MapUpgrade
    {
        private const string ShotgunModelName = "models/w_shotgun.mdl";
        private static readonly Vector3 AngleAdjust = new(0, 180, 90);
        private static readonly Vector3 Security2AngleAdjust = new(270, 180, 0);

        protected override void ApplyCore(MapUpgradeContext context)
        {
            if (context.GameInfo != ValveGames.OpposingForce && context.GameInfo != ValveGames.BlueShift)
            {
                return;
            }

            foreach (var entity in context.Map.Entities.OfClass("weapon_shotgun"))
            {
                UpdateAngles(entity, AngleAdjust);
                UpdateOrigin(entity, entity.GetOrigin());
            }

            if (context.Map.BaseName == "ba_security2")
            {
                foreach (var script in context.Map.Entities
                    .OfClass(ScriptedSequenceUtilities.ClassName)
                    .Where(e => e.ContainsKey(ScriptedSequenceUtilities.TargetKey))
                    .ToList())
                {
                    var shotgun = context.Map.Entities
                        .WhereTargetName(script.GetString(ScriptedSequenceUtilities.TargetKey))
                        .FirstOrDefault();

                    if (shotgun is null
                        || shotgun.ClassName != "monster_generic"
                        || shotgun.GetModel() != ShotgunModelName)
                    {
                        continue;
                    }

                    shotgun.ClassName = "item_generic";
                    //Wipe all spawnflags since they don't match between the two.
                    shotgun.SetSpawnFlags(0);

                    UpdateAngles(shotgun, Security2AngleAdjust);
                    UpdateOrigin(shotgun, script.GetOrigin());

                    //No longer needed.
                    context.Map.Entities.Remove(script);
                }
            }
        }

        private static void UpdateAngles(Entity entity, Vector3 adjust)
        {
            var angles = entity.GetAngles();

            angles += adjust;

            entity.SetAngles(angles);
        }

        private static void UpdateOrigin(Entity entity, Vector3 newOrigin)
        {
            //Adjust the vertical position by 2 units to match the original model.
            var angles = entity.GetAngles();

            MathUtilities.AngleVectors(angles, out _, out _, out var up);

            newOrigin += up * -2;

            entity.SetOrigin(newOrigin);
        }
    }
}
