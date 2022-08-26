using HalfLife.UnifiedSdk.Utilities.Tools;
using Serilog;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    internal sealed class LoggingMapEntity : IMapEntity
    {
        private readonly Map _map;
        private readonly ILogger _logger;

        public IMapEntity Entity { get; }

        public ImmutableDictionary<string, string> KeyValues => Entity.KeyValues;

        public bool IsWorldspawn => Entity.IsWorldspawn;

        public LoggingMapEntity(IMapEntity entity, Map map, ILogger logger)
        {
            Entity = entity;
            _map = map;
            _logger = logger;
        }

        public void SetKeyValue(string key, string value)
        {
            _logger.Information("[{ClassName}, {Index}] Setting {Key} to {Value}",
                KeyValues[KeyValueUtilities.ClassName], _map.IndexOf(this), key, value);
            Entity.SetKeyValue(key, value);
        }

        public void RemoveKeyValue(string key)
        {
            _logger.Information("[{ClassName}, {Index}] Removing {Key}",
                KeyValues[KeyValueUtilities.ClassName], _map.IndexOf(this), key);
            Entity.RemoveKeyValue(key);
        }

        public void RemoveAllKeyValues()
        {
            _logger.Information("[{ClassName}, {Index}] Removing all keyvalues",
                KeyValues[KeyValueUtilities.ClassName], _map.IndexOf(this));
            Entity.RemoveAllKeyValues();
        }
    }
}
