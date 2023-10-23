using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Converts the <c>monster_otis</c> model and body value to the appropriate keyvalues.
    /// </summary>
    internal sealed class ConvertOtisModelUpgrade : MapUpgrade
    {
        private const string OtisModelName = "models/otis.mdl";
        private const string IntroOtisModelName = "models/intro_otis.mdl";

        private enum OtisSleeves
        {
            Random = -1,
            Long = 0,
            Short
        }

        private enum OtisSkin
        {
            Random = -1,
            HeadWithHair = 0,
            Bald,
            BlackHeadWithHair
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var otis in context.Map.Entities.OfClass("monster_otis"))
            {
                var head = otis.GetInteger("head");

                otis.Remove("head");
                //Remove this because it will just screw up the submodel state otherwise.
                otis.Remove("body");
                //Previously unused, but now exist.
                otis.Remove("sleeves");
                otis.Remove("item");

                otis.SetInteger("skin", head);

                var sleeves = OtisSleeves.Long;

                if (head == (int)OtisSkin.Bald)
                {
                    sleeves = OtisSleeves.Short;
                }

                otis.SetInteger("sleeves", (int)sleeves);
            }

            foreach (var otis in context.Map.Entities.OfClass("monster_otis_dead"))
            {
                if (!otis.ContainsKey("body"))
                {
                    continue;
                }

                var body = otis.GetInteger("body");

                var bodystate = body switch
                {
                    1 => 2, // drawn
                    2 => 0, // donut (blank)
                    _ => 1 // holstered
                };

                otis.SetInteger("bodystate", bodystate);

                if (body == 2)
                {
                    // donut
                    otis.SetInteger("item", 2);
                }

                otis.Remove("body");
            }

            //Only Blue Shift uses Otis models separately from monster_otis so this section only applies to that game.
            if (context.GameInfo == ValveGames.BlueShift)
            {
                foreach (var entity in context.Map.Entities.WhereString("model", OtisModelName))
                {
                    var body = entity.GetInteger("body");

                    //Remap old submodels to new ones. Hardcoded to the body values used in Blue Shift to keep things simple.
                    body = body switch
                    {
                        2 => 13,
                        _ => 1,
                    };

                    entity.SetInteger("body", body);
                    entity.SetInteger("skin", (int)OtisSkin.Bald);
                }

                foreach (var entity in context.Map.Entities.WhereString("model", IntroOtisModelName))
                {
                    //Use the main Otis model for these.
                    entity.SetModel(OtisModelName);

                    entity.SetInteger("body", 9);
                    entity.SetInteger("skin", (int)OtisSkin.Bald);
                }
            }
        }
    }
}
