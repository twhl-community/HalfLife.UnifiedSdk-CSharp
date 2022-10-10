using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.BlueShift
{
    /// <summary>
    /// Updates references to specific sentences to use the correct vanilla Half-Life sentence.
    /// </summary>
    internal sealed class ChangeBlueShiftSentencesUpgrade : IMapUpgradeAction
    {
        private static readonly ImmutableDictionary<string, string> SentenceMap = new Dictionary<string, string>
        {
            // The NA group is for No Access, EA is for Enable Access.
            // BS incorrectly adds an access granted sentence to the NA group.
            { "!NA1", "!EA0" },
            // ba_security2 armory guard greets players as Freeman if using the original line.
            { "!BA_HELLO1", "!BSBA_HELLO1" },
        }.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);

        private static readonly ImmutableList<(string ClassName, string Key)> EntitiesToCheck = ImmutableList.Create(
            ("ambient_generic", "message"),
            ("scripted_sentence", "sentence")
            );

        public void Apply(MapUpgradeContext context)
        {
            if (context.GameInfo != ValveGames.BlueShift)
            {
                return;
            }

            foreach (var (className, key) in EntitiesToCheck)
            {
                foreach (var entity in context.Map.Entities.OfClass(className))
                {
                    if (entity.TryGetValue(key, out var value) && SentenceMap.TryGetValue(value, out var replacement))
                    {
                        entity.SetString(key, replacement);
                    }
                }
            }
        }
    }
}
