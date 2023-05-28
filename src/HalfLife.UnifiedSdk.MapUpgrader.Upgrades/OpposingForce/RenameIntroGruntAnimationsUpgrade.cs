using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.OpposingForce
{
    /// <summary>
    /// Renames the intro grunt animations.
    /// </summary>
    internal sealed class RenameIntroGruntAnimationsUpgrade : MapUpgrade
    {
        private record GruntData(string ModelName, ImmutableDictionary<string, string> AnimationRemap);

        private static readonly ImmutableArray<GruntData> GruntDatas = ImmutableArray.Create(
            new GruntData("models/intro_commander.mdl", new Dictionary<string, string>
            {
                { "fall_out", "intro_stand_fallout" },
                { "stand_react", "intro_stand_react" },
                { "bark", "intro_stand_bark" }
            }.ToImmutableDictionary()),
            new GruntData("models/intro_medic.mdl", Enumerable.Range(1, 7).ToImmutableDictionary(s => "sitting" + s, s => "intro_sitting_holster" + s)),
            new GruntData("models/intro_regular.mdl", Enumerable.Range(1, 5).ToImmutableDictionary(s => "sitting" + s, s => "intro_sitting_mp5_" + s)),
            new GruntData("models/intro_saw.mdl", Enumerable.Range(1, 5).ToImmutableDictionary(s => "sitting" + s, s => "intro_sitting_mp5_" + s)
                .Add("stiff", "intro_sitting_stiff")
                .Add("sit_React", "intro_sitting_react")
                .Add("cower", "intro_sitting_cower1")),
            new GruntData("models/intro_torch.mdl", Enumerable.Range(1, 7).ToImmutableDictionary(s => "sitting" + s, s => "intro_sitting_holster" + s)
                .Add("cower", "intro_sitting_cower2"))
            );

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var data in GruntDatas)
            {
                ScriptedSequenceUtilities.RenameAnimations(context, null, data.ModelName, data.AnimationRemap);
            }
        }
    }
}
