using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Converts the Opposing Force scientist <c>clipboard</c> and <c>stick</c> heads to use the <c>item</c> body group instead.
    /// </summary>
    internal sealed class ConvertScientistItemUpgrade : GameSpecificMapUpgrade
    {
        //This hardcoded stuff is pretty ugly, but there is no way around it without loading the model.
        private const int StudioCount = 1;
        private const int HeadsCount = 4;
        private const int NeedleCount = 2;

        enum ScientistItem
        {
            None = 0,
            Clipboard,
            Stick
        }

        public ConvertScientistItemUpgrade()
            : base(ValveGames.OpposingForce)
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var scientist in context.Map.Entities
                .Where(e => e.ClassName == "monster_scientist" && e.ContainsKey("body")))
            {
                var body = scientist.GetInteger("body");

                var (newBody, item) = DetermineValues(body);

                scientist.SetInteger("item", (int)item);

                scientist.SetInteger("body", newBody);
            }

            //Update any generics that use the model.
            foreach (var monster in context.Map.Entities
                .OfClass("monster_generic")
                .Where(e => e.GetModel() == "models/scientist.mdl"))
            {
                var body = monster.GetInteger("body");

                var (newBody, item) = DetermineValues(body);

                newBody += StudioCount * HeadsCount * NeedleCount * (int)item;

                monster.SetInteger("body", newBody);
            }
        }

        private static (int, ScientistItem) DetermineValues(int body)
        {
            return body switch
            {
                4 => (1, ScientistItem.Clipboard),
                5 => (3, ScientistItem.Stick),
                _ => (body, ScientistItem.None)
            };
        }
    }
}
