using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Converts the <c>gametitle</c> keyvalue to a string containing the game name.
    /// </summary>
    internal sealed class ConvertWorldspawnGameTitleValueUpgrade : IMapUpgradeAction
    {
        private const string GameTitleKey = "gametitle";

        public void Apply(MapUpgradeContext context)
        {
            var worldspawn = context.Map.Entities.Worldspawn;

            //Although the game uses a spawnflag to track this internally,
            //since it's undocumented and not used in official games it's not checked here.
            if (worldspawn.ContainsKey(GameTitleKey))
            {
                var value = worldspawn.GetInteger(GameTitleKey);

                if (value != 0)
                {
                    var titleToUse = context.Map.BaseName switch
                    {
                        "of0a0" => ValveGames.OpposingForce.ModDirectory,
                        "ba_tram1" => ValveGames.BlueShift.ModDirectory,
                        //For c0a0, custom maps, and anything else.
                        _ => ValveGames.HalfLife1.ModDirectory
                    };

                    worldspawn.SetString(GameTitleKey, titleToUse);
                }
                else
                {
                    //Disabled; remove the key.
                    worldspawn.Remove(GameTitleKey);
                }
            }
        }
    }
}
