namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades
{
    /// <summary>
    /// Log output diagnostics levels.
    /// </summary>
    public enum DiagnosticsLevel
    {
        /// <summary>
        /// No diagnostics output.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// All diagnostics output, excluding verbose output.
        /// </summary>
        Common,

        /// <summary>
        /// All diagnostics output.
        /// </summary>
        All
    }
}
