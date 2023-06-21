using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Updates references to specific sentences to use the correct vanilla Half-Life sentence.
    /// </summary>
    internal sealed class ChangeBlueShiftSentencesUpgrade : MapUpgrade
    {
        private static readonly ImmutableDictionary<string, string> SentenceMap = new Dictionary<string, string>
        {
            // The NA group is for No Access, EA is for Enable Access.
            // BS incorrectly adds an access granted sentence to the NA group.
            { "!NA1", "!EA0" },
            // ba_tram2 uses these two; they don't exist in Blue Shift,
            // but do in Half-Life and refer to Freeman and the HEV suit.
            { "!SC_HELLO6", "NULLSENT" },
            { "!SC_HELLO8", "NULLSENT" },
        }
        // Renames HOLO_* sentences to BSHOLO_*.
        .Concat(new[]
        {
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
        }.Select(s => new KeyValuePair<string, string>("!" + s, "!BS" + s)))
        .ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);

        protected override void ApplyCore(MapUpgradeContext context)
        {
            if (context.GameInfo != ValveGames.BlueShift)
            {
                return;
            }

            SentenceUtilities.ReplaceSentences(context.Map.Entities, SentenceMap);
        }
    }
}
