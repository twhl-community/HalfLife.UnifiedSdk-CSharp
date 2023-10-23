using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using Semver;
using Serilog;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    /// <summary>
    /// Factory for creating an upgrade tool.
    /// </summary>
    public static class MapUpgradeToolFactory
    {
        /// <summary>
        /// Creates an upgrade tool that applies the upgrades needed to upgrade a map to the latest version of the Unified SDK.
        /// </summary>
        public static MapUpgradeTool Create(ILogger logger, DiagnosticsLevel diagnosticsLevel)
        {
            return MapUpgradeToolBuilder.Build(builder =>
            {
                builder.AddUpgrades(new SemVersion(1, 0, 0), upgrade =>
                {
                    upgrade
                        .AddSharedUpgrades()
                        .AddHalfLifeUpgrades()
                        .AddOpposingForceUpgrades()
                        .AddBlueShiftUpgrades();
                });

                builder.WithDiagnostics(logger, diagnosticsBuilder =>
                {
                    if (diagnosticsLevel != DiagnosticsLevel.Disabled)
                    {
                        diagnosticsBuilder.WithAllEventTypes();

                        if (diagnosticsLevel != DiagnosticsLevel.All)
                        {
                            diagnosticsBuilder.IgnoreKeys("angle", "angles", MapUpgradeTool.DefaultGameVersionKey);
                        }
                    }
                });
            });
        }
    }
}
