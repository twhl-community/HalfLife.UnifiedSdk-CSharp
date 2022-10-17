using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    internal sealed class BaHazardRenameHoloSentencesUpgrade : MapSpecificUpgradeAction
    {
        private const string SentenceNameKey = "sentence";
        private const string SoundNameKey = "message";
        private const string BSPrefix = "!BS";

        private static readonly ImmutableHashSet<string> SentenceNames = ImmutableHashSet.Create(
            StringComparer.InvariantCultureIgnoreCase,
            "HOLO_4JUMPS",
            "HOLO_ARMOR",
            "HOLO_BREAKBOX",
            "HOLO_BREATH",
            "HOLO_BUTTON",
            "HOLO_CHARGER",
            "HOLO_COMMENCING",
            "HOLO_CONGRATS",
            "HOLO_DONE",
            "HOLO_DROWN",
            "HOLO_DUCK",
            "HOLO_FALLSHORT",
            "HOLO_FANTASTIC",
            "HOLO_FLASHLIGHT",
            "HOLO_GREATWORK",
            "HOLO_GRENADE",
            "HOLO_HITALL",
            "HOLO_INJURY",
            "HOLO_INTRO",
            "HOLO_JDUCK",
            "HOLO_JUMP",
            "HOLO_JUMPDOWN",
            "HOLO_JUMPGAP",
            "HOLO_KEEPTRYING",
            "HOLO_LADDER",
            "HOLO_LEADGUARD",
            "HOLO_LIGHTOFF",
            "HOLO_LONGJUMP",
            "HOLO_MEDKIT",
            "HOLO_MOVE",
            "HOLO_NICEJOB",
            "HOLO_PIPEDUCK",
            "HOLO_PULLBOX",
            "HOLO_PUSHBOX",
            "HOLO_RADIATION",
            "HOLO_RETRY",
            "HOLO_RUNSTART",
            "HOLO_SPINBRIDGE",
            "HOLO_STARTLIFT",
            "HOLO_STEAM",
            "HOLO_TARGET",
            "HOLO_TRYAGAIN",
            "HOLO_USETRAIN"
            );

        public BaHazardRenameHoloSentencesUpgrade()
            : base(Enumerable.Range(1, 6).Select(i => "ba_hazard" + i))
        {
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            foreach (var scriptedSentence in context.Map.Entities.OfClass("scripted_sentence"))
            {
                ReplaceSentences(scriptedSentence, SentenceNameKey);
            }

            foreach (var ambientGeneric in context.Map.Entities.OfClass("ambient_generic"))
            {
                ReplaceSentences(ambientGeneric, SoundNameKey);
            }
        }

        private static void ReplaceSentences(Entity entity, string key)
        {
            if (entity.TryGetValue(key, out var name) && name.StartsWith("!"))
            {
                name = name[1..];

                if (SentenceNames.Contains(name))
                {
                    entity.SetString(key, BSPrefix + name);
                }
            }
        }
    }
}
