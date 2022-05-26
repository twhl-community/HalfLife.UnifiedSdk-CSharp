using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    /// <summary>
    /// Renames the messages used in <c>env_message</c> entities to use a game-specific prefix.
    /// </summary>
    internal sealed class RenameMessagesUpgrade : IMapUpgradeAction
    {
        private const string MessageKey = "message";

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

        public void Apply(MapUpgradeContext context)
        {
            var prefix = context.GameName.ToUpperInvariant() + '_';

            foreach (var envMessage in context.Map.Entities.OfClass("env_message"))
            {
                if (envMessage.TryGetValue(MessageKey, out var message))
                {
                    if (Patterns.Any(p => p.IsMatch(message)))
                    {
                        envMessage.SetString(MessageKey, prefix + message);
                    }
                }
            }
        }
    }
}
