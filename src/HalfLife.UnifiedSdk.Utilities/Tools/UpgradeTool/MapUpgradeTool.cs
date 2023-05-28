using HalfLife.UnifiedSdk.Utilities.Logging.MapDiagnostics;
using HalfLife.UnifiedSdk.Utilities.Maps;
using Semver;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Tool to aid in the upgrading of maps from one version of a game to another.
    /// This tool invokes a set of upgrades registered to versions of a game.
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

        private readonly MapDiagnosticsEngine _diagnosticsEngine;

        /// <summary>Gets a sorted immutable list containing the upgrades used by this tool.</summary>
        public ImmutableList<MapUpgradeCollection> Upgrades { get; }

        /// <summary>
        /// Latest version to upgrade to.
        /// Defaults to the upgrade with the newest version.
        /// </summary>
        /// <remarks>
        /// If no upgrades were provided this will be <see cref="FirstVersion"/>.
        /// If the last upgrade is not the latest version of the game, set this to the latest version to ensure upgrades work properly.
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

        internal MapUpgradeTool(ImmutableList<MapUpgradeCollection> upgrades, MapDiagnosticsEngine diagnosticsEngine)
        {
            Upgrades = upgrades;
            _diagnosticsEngine = diagnosticsEngine;

            //If there are no upgrades then we can't do anything anyway, so use the first version.
            LatestVersion = Upgrades.LastOrDefault()?.Version ?? FirstVersion;
        }

        /// <summary>
        /// Performs the upgrade of the given map from the given current version to the given target version.
        /// </summary>
        /// <param name="command">Map upgrade command.</param>
        /// <returns>The version the map was upgraded from and the version the map was upgraded to.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> or <c>command.Map</c> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// If the map contains a <c>UpgradeToolVersion</c> value that is not a valid semantic version string.
        /// </exception>
        /// <exception cref="MapUpgradeException">
        /// If the version to upgrade from is older than the map version
        /// and <see cref="MapUpgradeCommand.ThrowOnTooOldVersion"/> is <see langword="true"/>.
        /// </exception>
        public (SemVersion From, SemVersion To) Upgrade(MapUpgradeCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            var currentVersion = GetVersion(command.Map);

            var from = command.From ?? currentVersion;
            var to = command.To ?? LatestVersion;

            if (command.ThrowOnTooOldVersion && from.ComparePrecedenceTo(currentVersion) < 0)
            {
                throw new MapUpgradeException(
                    $"Attempted to upgrade map with version {currentVersion} from older version {from} to version {to}");
            }

            //If this map is newer than what we know of, do nothing.
            //If this happens the map has probably been upgraded already by a newer version of the script or tool being used.
            //If that's the case then the upgrades may not work at all when applied.
            if (from.ComparePrecedenceTo(to) > 0)
            {
                return (from, from);
            }

            try
            {
                _diagnosticsEngine.AddTo(command.Map);

                //Apply all upgrades starting at currentVersion + 1 up to and including targetVersion.
                foreach (var upgrade in Upgrades.Where(v => v.Version.ComparePrecedenceTo(from) > 0 && v.Version.ComparePrecedenceTo(to) <= 0))
                {
                    var context = new MapUpgradeContext(this, from, to, currentVersion, upgrade, command.Map, command.GameInfo);
                    upgrade.PerformUpgrade(context);
                }

                SetVersion(command.Map, to);
            }
            finally
            {
                _diagnosticsEngine.RemoveFrom(command.Map);
            }

            return (from, to);
        }

        /// <summary>Tries to get the upgrade version of the given map.</summary>
        /// <remarks>Uses <see cref="GameVersionKey"/> to get the version.</remarks>
        public bool TryGetVersion(Map map, [MaybeNullWhen(false)] out SemVersion version)
        {
            ArgumentNullException.ThrowIfNull(map);

            if (map.Entities.Worldspawn.TryGetValue(GameVersionKey, out var currentVersionString))
            {
                version = SemVersion.Parse(currentVersionString, SemVersionStyles.Strict);
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
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(version);

            map.Entities.Worldspawn[GameVersionKey] = version.ToString();
        }
    }
}
