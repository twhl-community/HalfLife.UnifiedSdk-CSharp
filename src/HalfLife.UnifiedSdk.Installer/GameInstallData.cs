using HalfLife.UnifiedSdk.Utilities.Games;

namespace HalfLife.UnifiedSdk.Installer
{
    /// <summary>Defines a single game as a source of content.</summary>
    /// <param name="Info"><see cref="GameInfo"/> object containing information about the game.</param>
    /// <param name="MapEntFiles">
    /// If not null, the name of a a <c>.zip</c> archive containing <c>.ent</c> files to apply to the maps being copied.
    /// </param>
    sealed record class GameInstallData(GameInfo Info, string? MapEntFiles = null);
}
