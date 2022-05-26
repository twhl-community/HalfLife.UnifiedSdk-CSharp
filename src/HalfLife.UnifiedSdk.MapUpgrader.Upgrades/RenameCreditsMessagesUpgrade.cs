using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    /// <summary>
    /// Renames the credits messages used in <c>env_message</c> entities to use a game-specific prefix.
    /// </summary>
    internal sealed class RenameCreditsMessagesUpgrade : IMapUpgradeAction
    {
        private const string MessageKey = "message";

        private static readonly Regex CreditsRegex = new(@"^CR\d+$");

        public void Apply(MapUpgradeContext context)
        {
            foreach (var envMessage in context.Map.Entities.OfClass("env_message"))
            {
                if (envMessage.TryGetValue(MessageKey, out var message) && CreditsRegex.IsMatch(message))
                {
                    envMessage.SetString(MessageKey, context.GameName + message);
                }
            }
        }
    }
}
