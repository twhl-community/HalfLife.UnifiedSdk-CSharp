using HalfLife.UnifiedSdk.Utilities.Maps;
using Semver;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Tool to aid in the upgrading of maps from one version of a game to another.
    /// This tool will invoke a set of actions registered to versions of a game.
    /// </summary>
    /// <example>
    /// A map made for version <c>1.0.0</c> upgraded to version <c>3.0.0</c> will have upgrades applied for versions
    /// starting from the version after <c>1.0.0</c> up to and including <c>3.0.0</c>.
    /// The map will have a keyvalue <c>UpgradeToolVersion</c> added containing the version it was upgraded to.
    /// </example>
    public sealed class MapUpgradeTool
    {
        /// <summary>Worldspawn keyvalue used to track which version of a game a map is made for.</summary>
        public const string DefaultGameVersionKey = "UpgradeToolVersion";

        /// <summary>The first version that any map can be.</summary>
        public static readonly SemVersion FirstVersion = new(0, 0, 0);

        /// <summary>Gets a sorted immutable list containing the upgrade actions used by this tool.</summary>
        public ImmutableList<MapUpgradeAction> Actions { get; }

        /// <summary>
        /// Latest version to upgrade to.
        /// Defaults to the last upgrade action provided.
        /// </summary>
        /// <remarks>
        /// If no upgrade actions were provided this will be <see cref="FirstVersion"/>.
        /// If the last upgrade action is not the latest version of the game, set this to the latest version to ensure upgrades work properly.
        /// </remarks>
        public SemVersion LatestVersion { get; init; }

        private readonly string _gameVersionKey = DefaultGameVersionKey;

        /// <summary>
        /// The <c>worldspawn</c> keyvalue key used to track the version of the game the map is made for.
        /// Defaults to <see cref="DefaultGameVersionKey"/>
        /// </summary>
        public string GameVersionKey
        {
            get => _gameVersionKey;

            init
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Game version key must be a valid key", nameof(value));
                }

                _gameVersionKey = value;
            }
        }

        /// <summary>Creates a new instance of the upgrade tool with the given map upgrade actions.</summary>
        /// <param name="actions">Set of actions to perform to upgrade a map.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actions"/> is <see langword="null"/>.</exception>
        public MapUpgradeTool(IEnumerable<MapUpgradeAction> actions)
        {
            if (actions is null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            Actions = actions.ToImmutableList().Sort();

            if (Actions.Count != actions.Distinct().Count())
            {
                throw new ArgumentException("Only one action may be associated with a specific version", nameof(actions));
            }

            //If there are no upgrades then we can't do anything anyway, so use the first version.
            LatestVersion = Actions.LastOrDefault()?.Version ?? FirstVersion;
        }

        /// <inheritdoc cref="MapUpgradeTool(IEnumerable{MapUpgradeAction})"/>
        public MapUpgradeTool(params MapUpgradeAction[] actions)
            : this((IEnumerable<MapUpgradeAction>)actions)
        {
        }

        /// <summary>
        /// Performs the upgrade of the given map from the given current version to the given target version.
        /// </summary>
        /// <param name="mapUpgrade">Map upgrade command.</param>
        /// <returns>The version the map was upgraded from and the version the map was upgraded to.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mapUpgrade"/> or <c>mapUpgrade.Map</c> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// If the map contains a <c>UpgradeToolVersion</c> value that is not a valid semantic version string.
        /// </exception>
        /// <exception cref="MapUpgradeException">
        /// If the version to upgrade from is older than the map version
        /// and <see cref="MapUpgrade.ThrowOnTooOldVersion"/> is <see langword="true"/>.
        /// </exception>
        public (SemVersion From, SemVersion To) Upgrade(MapUpgrade mapUpgrade)
        {
            if (mapUpgrade is null)
            {
                throw new ArgumentNullException(nameof(mapUpgrade));
            }

            if (mapUpgrade.Map is null)
            {
                throw new ArgumentNullException(nameof(mapUpgrade));
            }

            var currentVersion = GetVersion(mapUpgrade.Map);

            var from = mapUpgrade.From ?? currentVersion;
            var to = mapUpgrade.To ?? LatestVersion;

            if (mapUpgrade.ThrowOnTooOldVersion && from < currentVersion)
            {
                throw new MapUpgradeException(
                    $"Attempted to upgrade map with version {currentVersion} from older version {from} to version {to}");
            }

            //If this map is newer than what we know of, do nothing.
            //If this happens the map has probably been upgraded already by a newer version of the script or tool being used.
            //If that's the case then the upgrades may not work at all when applied.
            if (from > to)
            {
                return (from, from);
            }

            //Apply all upgrades starting at currentVersion + 1 up to and including targetVersion.
            foreach (var upgradeAction in Actions.Where(v => v.Version > from && v.Version <= to))
            {
                var context = new MapUpgradeContext(this, from, to, currentVersion, upgradeAction, mapUpgrade.Map);
                upgradeAction.PerformUpgrade(context);
            }

            SetVersion(mapUpgrade.Map, to);

            return (from, to);
        }

        /// <summary>Tries to get the upgrade version of the given map.</summary>
        /// <remarks>Uses <see cref="GameVersionKey"/> to get the version.</remarks>
        public bool TryGetVersion(Map map, [MaybeNullWhen(false)] out SemVersion version)
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            if (map.Entities.Worldspawn.TryGetValue(GameVersionKey, out var currentVersionString))
            {
                version = SemVersion.Parse(currentVersionString, true);
                return true;
            }

            version = null;
            return false;
        }

        /// <summary>
        /// Gets the game version of the given map, or <see cref="FirstVersion"/> if no version can be found.
        /// </summary>
        /// <remarks>Uses <see cref="GameVersionKey"/> to get the version.</remarks>
        public SemVersion GetVersion(Map map)
        {
            if (TryGetVersion(map, out var version))
            {
                return version;
            }

            return FirstVersion;
        }

        /// <summary>Updates or adds the game version to the given map.</summary>
        /// <remarks>Uses <see cref="GameVersionKey"/> to set the version.</remarks>
        public void SetVersion(Map map, SemVersion version)
        {
            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            if (version is null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            map.Entities.Worldspawn[GameVersionKey] = version.ToString();
        }
    }
}
