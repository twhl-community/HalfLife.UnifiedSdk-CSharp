using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.Installer
{
    /// <summary>Defines a single game as a source of content.</summary>
    /// <param name="Info"><see cref="GameInfo"/> object containing information about the game.</param>
    /// <param name="GetUpgradeTool">Delegate that provides an upgrade tool to apply to the game's maps.</param>
    /// <param name="AdditionalCopySteps">
    /// If not null, this action will be invoked to perform any additional copy steps.
    /// Provides the source and destination mod directories</param>
    internal sealed record class GameInstallData(
        GameInfo Info,
        Func<MapUpgradeTool> GetUpgradeTool,
        Action<string, string>? AdditionalCopySteps = null);
}
