using Serilog;
using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Logging.MapDiagnostics
{
    /// <summary>Builds map diagnostics engines.</summary>
    public sealed class MapDiagnosticsBuilder
    {
        private DiagnosticsEventTypes _eventTypes = DiagnosticsEventTypes.None;

        private readonly ImmutableHashSet<string>.Builder _keysToIgnore =
            ImmutableHashSet.CreateBuilder<string>(StringComparer.OrdinalIgnoreCase);

        internal MapDiagnosticsBuilder()
        {
        }

        internal MapDiagnosticsEngine Build(ILogger logger, Action<MapDiagnosticsBuilder>? configurator)
        {
            ArgumentNullException.ThrowIfNull(logger);

            configurator?.Invoke(this);

            return new MapDiagnosticsEngine(logger, _eventTypes, _keysToIgnore.ToImmutable());
        }

        /// <summary>Sets the enabled event types to the given types.</summary>
        public MapDiagnosticsBuilder WithEventTypes(DiagnosticsEventTypes eventTypes)
        {
            _eventTypes = eventTypes;
            return this;
        }

        /// <summary>Enables the given event types.</summary>
        public MapDiagnosticsBuilder EnableEventTypes(DiagnosticsEventTypes eventTypes)
        {
            _eventTypes |= eventTypes;
            return this;
        }

        /// <summary>Disables the given event types.</summary>
        public MapDiagnosticsBuilder DisableEventTypes(DiagnosticsEventTypes eventTypes)
        {
            _eventTypes &= ~eventTypes;
            return this;
        }

        /// <summary>Enables all event types.</summary>
        public MapDiagnosticsBuilder WithAllEventTypes() => WithEventTypes(DiagnosticsEventTypes.All);

        /// <summary>Disables all event types.</summary>
        public MapDiagnosticsBuilder WithNoEventTypes() => WithEventTypes(DiagnosticsEventTypes.None);

        /// <summary>Adds the given keys to the set of keys to ignore.</summary>
        public MapDiagnosticsBuilder IgnoreKeys(params string[] keys)
        {
            _keysToIgnore.UnionWith(keys);
            return this;
        }
    }
}
