using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Renames the messages used in <c>env_message</c> entities and <c>worldspawn</c> to use a game-specific prefix.
    /// </summary>
    internal sealed class RenameMessagesUpgrade : MapUpgrade
    {
        private const string MessageKey = "message";
        private const string WorldspawnKey = "chaptertitle";

        private static readonly ImmutableArray<string> ClassNames = ImmutableArray.Create(
            "env_message",
            "player_loadsaved"
            );

        private static readonly ImmutableList<Regex> Patterns = ImmutableList.Create(
            new Regex(@"^CR\d+$"),
            new Regex(@"^END\d+$"),
            new Regex("^GAMEOVER$"),
            new Regex("^TRAITOR$"),
            new Regex("^LOSER$"),
            new Regex("^GAMETITLE$"),
            new Regex("^T0A0TITLE$"),
            new Regex("^HZBUTTON1$"),
            new Regex("^HZBARNEY$")
            );

        protected override void ApplyCore(MapUpgradeContext context)
        {
            var prefix = context.GameInfo.ModDirectory.ToUpperInvariant() + '_';

            void CheckMessage(Entity entity, string key)
            {
                if (entity.TryGetValue(key, out var message)
                    && Patterns.Any(p => p.IsMatch(message)))
                {
                    entity.SetString(key, prefix + message);
                }
            }

            foreach (var entity in context.Map.Entities.Where(e => ClassNames.Contains(e.ClassName)))
            {
                CheckMessage(entity, MessageKey);
            }

            // Worldspawn creates an env_message to handle this, so make sure it also gets converted.
            CheckMessage(context.Map.Entities.Worldspawn, WorldspawnKey);
        }
    }
}
