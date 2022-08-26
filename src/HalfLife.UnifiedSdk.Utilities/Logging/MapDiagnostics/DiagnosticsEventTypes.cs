using System;

namespace HalfLife.UnifiedSdk.Utilities.Logging.MapDiagnostics
{
    /// <summary>Bit flags to indicate which diagnostics events to log.</summary>
    [Flags]
    public enum DiagnosticsEventTypes
    {
        /// <summary>Don't log anything.</summary>
        None = 0,

        /// <summary>Log entity created events.</summary>
        EntityCreated = 1 << 0,

        /// <summary>Log entity removed events.</summary>
        EntityRemoved = 1 << 1,

        /// <summary>Log keyvalue added events.</summary>
        KeyValueAdded = 1 << 2,

        /// <summary>Log keyvalue changed events.</summary>
        KeyValueChanged = 1 << 3,

        /// <summary>Log keyvalue removed events.</summary>
        KeyValueRemoved = 1 << 4,

        /// <summary>Log all keyvalues removed events.</summary>
        AllKeyValuesRemoved = 1 << 5,

        /// <summary>Log all events.</summary>
        All = EntityCreated | EntityRemoved | KeyValueAdded | KeyValueChanged | KeyValueRemoved | AllKeyValuesRemoved
    }
}
