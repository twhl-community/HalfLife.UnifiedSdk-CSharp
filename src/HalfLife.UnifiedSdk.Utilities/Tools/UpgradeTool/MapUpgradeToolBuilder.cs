using HalfLife.UnifiedSdk.Utilities.Logging.MapDiagnostics;
using Semver;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool
{
    /// <summary>
    /// Builds <see cref="MapUpgradeTool"/> objects.
    /// </summary>
    public sealed class MapUpgradeToolBuilder
    {
        private readonly ImmutableList<MapUpgradeCollection>.Builder _upgrades = ImmutableList.CreateBuilder<MapUpgradeCollection>();

        private MapDiagnosticsEngine? _diagnosticsEngine;

        private MapUpgradeToolBuilder()
        {
        }

        /// <summary>
        /// Builds a map upgrade tool by invoking a callback which populates the list of upgrades.
        /// </summary>
        /// <param name="callback">Callback to invoke.</param>
        public static MapUpgradeTool Build(Action<MapUpgradeToolBuilder> callback)
        {
            ArgumentNullException.ThrowIfNull(callback);

            MapUpgradeToolBuilder builder = new();

            callback(builder);

            return builder.BuildCore();
        }

        private MapUpgradeTool BuildCore()
        {
            //Build a default engine that does nothing.
            _diagnosticsEngine ??= MapDiagnosticsEngine.Create(Logger.None, _ => { });

            return new MapUpgradeTool(_upgrades.ToImmutable().Sort(), _diagnosticsEngine);
        }

        /// <summary>
        /// Adds an upgrade to the tool.
        /// </summary>
        /// <param name="version">Version to associate to this upgrade.</param>
        /// <param name="callback">Callback to invoke.</param>
        /// <exception cref="ArgumentException">If the given version is already used by an existing upgrade.</exception>
        public MapUpgradeToolBuilder AddUpgrades(SemVersion version, Action<MapUpgradeCollectionBuilder> callback)
        {
            ArgumentNullException.ThrowIfNull(version);
            ArgumentNullException.ThrowIfNull(callback);

            if (_upgrades.Find(u => u.Version == version) is not null)
            {
                throw new ArgumentException("Only one upgrade may be associated with a specific version", nameof(version));
            }

            MapUpgradeCollectionBuilder builder = new();

            callback(builder);

            _upgrades.Add(builder.Build(version));

            return this;
        }

        /// <summary>Adds a diagnostics engine to use.</summary>
        /// <exception cref="InvalidOperationException">If there is already a diagnostics engine.</exception>
        /// <seealso cref="MapDiagnosticsEngine.Create(ILogger, Action{MapDiagnosticsBuilder}?)"/>
        public MapUpgradeToolBuilder WithDiagnostics(ILogger logger, Action<MapDiagnosticsBuilder>? configurator = null)
        {
            if (_diagnosticsEngine is not null)
            {
                throw new InvalidOperationException("Cannot add more than one diagnostics engine");
            }

            _diagnosticsEngine = MapDiagnosticsEngine.Create(logger, configurator);

            return this;
        }

        /// <summary>Adds a diagnostics engine to use.</summary>
        /// <exception cref="InvalidOperationException">If there is already a diagnostics engine.</exception>
        public MapUpgradeToolBuilder WithDiagnostics(MapDiagnosticsEngine diagnosticsEngine)
        {
            ArgumentNullException.ThrowIfNull(diagnosticsEngine);

            if (_diagnosticsEngine is not null)
            {
                throw new InvalidOperationException("Cannot add more than one diagnostics engine");
            }

            _diagnosticsEngine = diagnosticsEngine;

            return this;
        }
    }
}
