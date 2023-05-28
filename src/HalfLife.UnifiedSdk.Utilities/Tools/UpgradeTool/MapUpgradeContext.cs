using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Maps;
using Semver;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Context for map upgrade events.
    /// </summary>
    /// <remarks>
    /// To get the version being upgraded to by the currently executing delegate, use <see cref="MapUpgradeCollection.Version"/>
    /// </remarks>
    /// <param name="Tool">The tool performing the upgrade.</param>
    /// <param name="FromVersion">The version being upgraded from.</param>
    /// <param name="ToVersion">The version being upgraded.</param>
    /// <param name="OriginalVersion">
    /// The original version of the map, or <see cref="MapUpgradeTool.FirstVersion"/> if no version could be found.
    /// </param>
    /// <param name="Upgrade">The upgrade being applied.</param>
    /// <param name="Map">The map being upgraded.</param>
    /// <param name="GameInfo">Specifies which game the map is from.</param>
    public sealed record MapUpgradeContext(
        MapUpgradeTool Tool,
        SemVersion FromVersion,
        SemVersion ToVersion,
        SemVersion OriginalVersion,
        MapUpgradeCollection Upgrade,
        Map Map,
        GameInfo GameInfo);
}
