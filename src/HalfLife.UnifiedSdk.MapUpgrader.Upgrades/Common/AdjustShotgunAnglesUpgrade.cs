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
    internal sealed class AdjustShotgunAnglesUpgrade : IMapUpgradeAction
    {
        private const string ShotgunModelName = "models/w_shotgun.mdl";
        private const string ScriptedSequenceTargetKey = "m_iszEntity";

        public void Apply(MapUpgradeContext context)
        {
            if (!ValveGames.OpposingForce.IsMap(context.Map.BaseName) && !ValveGames.BlueShift.IsMap(context.Map.BaseName))
            {
                return;
            }

            foreach (var entity in context.Map.Entities.OfClass("weapon_shotgun"))
            {
                UpdateAngles(entity);
                UpdateOrigin(entity, entity.GetOrigin());
            }

            if (context.Map.BaseName == "ba_security2")
            {
                foreach (var script in context.Map.Entities
                    .OfClass("scripted_sequence")
                    .Where(e => e.ContainsKey(ScriptedSequenceTargetKey))
                    .ToList())
                {
                    var shotgun = context.Map.Entities.WhereTargetName(script.GetString(ScriptedSequenceTargetKey)).FirstOrDefault();

                    if (shotgun is null
                        || shotgun.ClassName != "monster_generic"
                        || shotgun.GetModel() != ShotgunModelName)
                    {
                        continue;
                    }

                    shotgun.ClassName = "item_generic";
                    //Wipe all spawnflags since they don't match between the two.
                    shotgun.SetSpawnFlags(0);

                    //No need to update angles; original angles are correct.
                    //UpdateAngles(shotgun);
                    UpdateOrigin(shotgun, script.GetOrigin());

                    //No longer needed.
                    context.Map.Entities.Remove(script);
                }
            }
        }

        private static void UpdateAngles(Entity entity)
        {
            var angles = entity.GetAngles();

            angles.Z += 90;
            angles.X += 180;

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
