using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Converts all entities that use sounds or sentences by index to use sound filenames or sentence names instead.
    /// </summary>
    internal sealed class ConvertSoundIndicesToNamesUpgrade : MapUpgrade
    {
        private record struct KeyData(string KeyName, string DefaultValue, ImmutableArray<string> Names, Func<Entity, string?>? ValueGetter = null);

        private const string DefaultSound = "common/null.wav";
        private const string DefaultSentence = "";
        private const string DefaultButtonSound = "";
        private const string DefaultMomentaryButtonSound = "buttons/button9.wav";
        private const string DefaultTrackTrainSound = "";

        private static readonly ImmutableArray<string> DoorMoveSounds = ImmutableArray.Create(
            "common/null.wav",
            "doors/doormove1.wav",
            "doors/doormove2.wav",
            "doors/doormove3.wav",
            "doors/doormove4.wav",
            "doors/doormove5.wav",
            "doors/doormove6.wav",
            "doors/doormove7.wav",
            "doors/doormove8.wav",
            "doors/doormove9.wav",
            "doors/doormove10.wav"
            );

        private static readonly ImmutableArray<string> DoorStopSounds = ImmutableArray.Create(
            "common/null.wav",
            "doors/doorstop1.wav",
            "doors/doorstop2.wav",
            "doors/doorstop3.wav",
            "doors/doorstop4.wav",
            "doors/doorstop5.wav",
            "doors/doorstop6.wav",
            "doors/doorstop7.wav",
            "doors/doorstop8.wav"
            );

        private static readonly ImmutableArray<string> ButtonSounds = ImmutableArray.Create(
            "common/null.wav",
            "buttons/button1.wav",
            "buttons/button2.wav",
            "buttons/button3.wav",
            "buttons/button4.wav",
            "buttons/button5.wav",
            "buttons/button6.wav",
            "buttons/button7.wav",
            "buttons/button8.wav",
            "buttons/button9.wav",
            "buttons/button10.wav",
            "buttons/button11.wav",
            "buttons/latchlocked1.wav",
            "buttons/latchunlocked1.wav",
            "buttons/lightswitch2.wav",
            "buttons/button9.wav",
            "buttons/button9.wav",
            "buttons/button9.wav",
            "buttons/button9.wav",
            "buttons/button9.wav",
            "buttons/button9.wav",
            "buttons/lever1.wav",
            "buttons/lever2.wav",
            "buttons/lever3.wav",
            "buttons/lever4.wav",
            "buttons/lever5.wav"
            );

        private static readonly ImmutableArray<string> ButtonLockedSentences = ImmutableArray.Create(
            "",
            "NA",
            "ND",
            "NF",
            "NFIRE",
            "NCHEM",
            "NRAD",
            "NCON",
            "NH",
            "NG"
            );

        private static readonly ImmutableArray<string> ButtonUnlockedSentences = ImmutableArray.Create(
            "",
            "EA",
            "ED",
            "EF",
            "EFIRE",
            "ECHEM",
            "ERAD",
            "ECON",
            "EH"
            );

        private static readonly ImmutableArray<KeyData> DoorData = ImmutableArray.Create<KeyData>(

            new("movesnd", DefaultSound, DoorMoveSounds),
            new("stopsnd", DefaultSound, DoorStopSounds),
            new("locked_sound", DefaultButtonSound, ButtonSounds),
            new("unlocked_sound", DefaultButtonSound, ButtonSounds),
            new("locked_sentence", DefaultSentence, ButtonLockedSentences),
            new("unlocked_sentence", DefaultSentence, ButtonUnlockedSentences)
        );

        private static readonly ImmutableArray<KeyData> ButtonData = ImmutableArray.Create<KeyData>(
            new("sounds", DefaultButtonSound, ButtonSounds),
            new("locked_sound", DefaultButtonSound, ButtonSounds),
            new("unlocked_sound", DefaultButtonSound, ButtonSounds),
            new("locked_sentence", DefaultSentence, ButtonLockedSentences),
            new("unlocked_sentence", DefaultSentence, ButtonUnlockedSentences)
        );

        private static readonly ImmutableArray<string> MomentaryDoorMoveSounds = ImmutableArray.Create(
            "common/null.wav",
            "doors/doormove1.wav",
            "doors/doormove2.wav",
            "doors/doormove3.wav",
            "doors/doormove4.wav",
            "doors/doormove5.wav",
            "doors/doormove6.wav",
            "doors/doormove7.wav",
            "doors/doormove8.wav"
            );

        private static readonly ImmutableArray<string> RotatingMoveSounds = ImmutableArray.Create(
            "common/null.wav",
            "fans/fan1.wav",
            "fans/fan2.wav",
            "fans/fan3.wav",
            "fans/fan4.wav",
            "fans/fan5.wav"
            );

        private static readonly ImmutableArray<string> PlatMoveSounds = ImmutableArray.Create(
            "common/null.wav",
            "plats/bigmove1.wav",
            "plats/bigmove2.wav",
            "plats/elevmove1.wav",
            "plats/elevmove2.wav",
            "plats/elevmove3.wav",
            "plats/freightmove1.wav",
            "plats/freightmove2.wav",
            "plats/heavymove1.wav",
            "plats/rackmove1.wav",
            "plats/railmove1.wav",
            "plats/squeekmove1.wav",
            "plats/talkmove1.wav",
            "plats/talkmove2.wav"
            );

        private static readonly ImmutableArray<string> PlatStopSounds = ImmutableArray.Create(
            "common/null.wav",
            "plats/bigstop1.wav",
            "plats/bigstop2.wav",
            "plats/freightstop1.wav",
            "plats/heavystop2.wav",
            "plats/rackstop1.wav",
            "plats/railstop1.wav",
            "plats/squeekstop1.wav",
            "plats/talkstop1.wav"
            );

        private static readonly ImmutableArray<KeyData> PlatData = ImmutableArray.Create<KeyData>(
            new("movesnd", DefaultButtonSound, PlatMoveSounds),
            new("stopsnd", DefaultButtonSound, PlatStopSounds)
        );

        private static readonly ImmutableArray<string> TrackTrainMoveSounds = ImmutableArray.Create(
            "",
            "plats/ttrain1.wav",
            "plats/ttrain2.wav",
            "plats/ttrain3.wav",
            "plats/ttrain4.wav",
            "plats/ttrain6.wav",
            "plats/ttrain7.wav"
            );

        private static readonly ImmutableDictionary<string, ImmutableArray<KeyData>> EntityData =
            new Dictionary<string, ImmutableArray<KeyData>>
            {
                ["func_door"] = DoorData,
                ["func_water"] = DoorData,
                ["func_door_rotating"] = DoorData,
                ["momentary_door"] = ImmutableArray.Create(new KeyData("movesnd", DefaultSound, MomentaryDoorMoveSounds)),
                ["func_rotating"] = ImmutableArray.Create(new KeyData("sounds", DefaultSound, RotatingMoveSounds, entity =>
                {
                    // Use original game parsing behavior.
                    var value = entity.GetStringOrNull("message");

                    entity.Remove("message");

                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }

                    return value;
                })),
                ["func_button"] = ButtonData,
                ["func_rot_button"] = ButtonData,
                ["momentary_rot_button"] = ImmutableArray.Create(new KeyData("sounds", DefaultMomentaryButtonSound, ButtonSounds)),
                ["func_train"] = PlatData,
                ["func_plat"] = PlatData,
                ["func_platrot"] = PlatData,
                ["func_trackchange"] = PlatData,
                ["func_trackautochange"] = PlatData,
                ["env_spritetrain"] = PlatData,
                ["func_tracktrain"] = ImmutableArray.Create(new KeyData("sounds", DefaultTrackTrainSound, TrackTrainMoveSounds))
            }.ToImmutableDictionary();

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var entity in context.Map.Entities)
            {
                if (EntityData.TryGetValue(entity.ClassName, out var data))
                {
                    foreach (var keyData in data)
                    {
                        // Allow handler to provide the value for us if needed.
                        var name = keyData.ValueGetter?.Invoke(entity);

                        if (name is null)
                        {
                            name = keyData.DefaultValue;

                            if (entity.TryGetValue(keyData.KeyName, out var value))
                            {
                                _ = int.TryParse(value, out var index);

                                if (index >= 0 && index < keyData.Names.Length)
                                {
                                    name = keyData.Names[index];
                                }
                            }
                        }

                        entity.Remove(keyData.KeyName);

                        if (name.Length > 0)
                        {
                            entity.SetString(keyData.KeyName, name);
                        }
                    }
                }
            }
        }
    }
}
