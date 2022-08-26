using HalfLife.UnifiedSdk.Utilities.Games;

namespace HalfLife.UnifiedSdk.Installer
{
    /// <summary>Defines a single game as a source of content.</summary>
    /// <param name="Info"><see cref="GameInfo"/> object containing information about the game.</param>
    /// <param name="AdditionalCopySteps">
    /// If not null, this action will be invoked to perform any additional copy steps.
    /// Provides the source and destination mod directories</param>
    internal sealed record class GameInstallData(
        GameInfo Info,
        Action<string, string>? AdditionalCopySteps = null);
}
