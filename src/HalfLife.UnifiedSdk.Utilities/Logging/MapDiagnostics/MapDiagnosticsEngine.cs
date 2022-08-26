using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Maps;
using Serilog;
using System;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Logging.MapDiagnostics
{
    /// <summary>
    /// Listens to events raised by a map's entity list and logs them.
    /// </summary>
    public sealed class MapDiagnosticsEngine
    {
        private readonly ILogger _logger;

        private readonly DiagnosticsEventTypes _eventTypes;

        private readonly ImmutableHashSet<string> _keysToIgnore;

        internal MapDiagnosticsEngine(ILogger logger, DiagnosticsEventTypes eventTypes, ImmutableHashSet<string> keysToIgnore)
        {
            _logger = logger;
            _eventTypes = eventTypes;
            _keysToIgnore = keysToIgnore;
        }

        /// <summary>Creates a new diagnostics engine.</summary>
        /// <remarks>
        /// The default configuration disables all event types, effectively logging nothing.
        /// </remarks>
        /// <param name="logger">Logger to use.</param>
        /// <param name="configurator">Callback to configure the engine, or <see langword="null"/> to use the default configuration.</param>
        /// <see cref="MapDiagnosticsBuilder"/>
        public static MapDiagnosticsEngine Create(ILogger logger, Action<MapDiagnosticsBuilder>? configurator = null)
        {
            return new MapDiagnosticsBuilder().Build(logger, configurator);
        }

        /// <summary>Adds diagnostics to the given map.</summary>
        public void AddTo(Map map)
        {
            ArgumentNullException.ThrowIfNull(map);

            var entities = map.Entities;

            if (_eventTypes.HasFlag(DiagnosticsEventTypes.EntityCreated))
            {
                entities.EntityCreated += Entities_EntityCreated;
            }

            if (_eventTypes.HasFlag(DiagnosticsEventTypes.EntityRemoved))
            {
                entities.RemovingEntity += Entities_RemovingEntity;
            }

            if (_eventTypes.HasFlag(DiagnosticsEventTypes.KeyValueAdded) || _eventTypes.HasFlag(DiagnosticsEventTypes.KeyValueChanged))
            {
                entities.KeyValueChanged += Entities_KeyValueChanged;
            }

            if (_eventTypes.HasFlag(DiagnosticsEventTypes.KeyValueRemoved))
            {
                entities.RemovingKeyValue += Entities_RemovingKeyValue;
            }

            if (_eventTypes.HasFlag(DiagnosticsEventTypes.AllKeyValuesRemoved))
            {
                entities.RemovingAllKeyValues += Entities_RemovingAllKeyValues;
            }
        }

        /// <summary>Removes diagnostics from the given map.</summary>
        public void RemoveFrom(Map map)
        {
            ArgumentNullException.ThrowIfNull(map);

            var entities = map.Entities;

            entities.EntityCreated -= Entities_EntityCreated;
            entities.RemovingEntity -= Entities_RemovingEntity;
            entities.KeyValueChanged -= Entities_KeyValueChanged;
            entities.RemovingKeyValue -= Entities_RemovingKeyValue;
            entities.RemovingAllKeyValues -= Entities_RemovingAllKeyValues;
        }

        private void Entities_EntityCreated(object? sender, EntityEventArgs e)
        {
            _logger.Information("[{$Entity}] Entity created", e.Entity);
        }

        private void Entities_RemovingEntity(object? sender, EntityEventArgs e)
        {
            _logger.Information("[{$Entity}] Entity removed", e.Entity);
        }

        private void Entities_KeyValueChanged(object? sender, EntityKeyValueChangedEventArgs e)
        {
            if (_keysToIgnore.Contains(e.Key))
            {
                return;
            }

            if (e.PreviousValue is not null)
            {
                if (_eventTypes.HasFlag(DiagnosticsEventTypes.KeyValueChanged))
                {
                    _logger.Information("[{$Entity}] KeyValue [{Key}:{PreviousValue}] changed to [{CurrentValue}]",
                    e.Entity, e.Key, e.PreviousValue, e.CurrentValue);
                }
            }
            else
            {
                if (_eventTypes.HasFlag(DiagnosticsEventTypes.KeyValueAdded))
                {
                    _logger.Information("[{$Entity}] KeyValue [{Key}:{CurrentValue}] added",
                    e.Entity, e.Key, e.CurrentValue);
                }
            }
        }

        private void Entities_RemovingKeyValue(object? sender, EntityKeyValueRemovingEventArgs e)
        {
            if (_keysToIgnore.Contains(e.Key))
            {
                return;
            }

            _logger.Information("[{$Entity}] KeyValue [{Key}:{Value}] removed", e.Entity, e.Key, e.Entity.GetStringOrNull(e.Key));
        }
        private void Entities_RemovingAllKeyValues(object? sender, EntityEventArgs e)
        {
            _logger.Information("[{$Entity}] All keyvalues removed", e.Entity);
        }
    }
}
