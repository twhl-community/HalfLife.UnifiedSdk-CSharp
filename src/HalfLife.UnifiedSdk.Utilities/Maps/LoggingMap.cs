using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools;
using Serilog;
using System.IO;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    internal sealed class LoggingMap : Map
    {
        private readonly Map _map;

        private readonly ILogger _logger;

        public override EntityList Entities { get; }

        public LoggingMap(Map map, ILogger logger)
            : base(map.FileName, map.ContentType)
        {
            _map = map;
            _logger = logger;
            Entities = new EntityList(this, _map.Entities.Select(e => new LoggingMapEntity(e._entity, _map, _logger)));
        }

        internal override IMapEntity CreateNewEntity(string className)
        {
            _logger.Verbose("Created entity of class {ClassName}", className);
            return new LoggingMapEntity(_map.CreateNewEntity(className), _map, _logger);
        }

        internal override int IndexOf(IMapEntity entity)
        {
            var loggingEntity = (LoggingMapEntity)entity;
            return _map.IndexOf(loggingEntity.Entity);
        }

        internal override void Add(IMapEntity entity)
        {
            var loggingEntity = (LoggingMapEntity)entity;
            _map.Add(loggingEntity.Entity);
            _logger.Verbose("[{ClassName}, {Index}] Adding entity", entity.KeyValues[KeyValueUtilities.ClassName], IndexOf(entity));
        }

        internal override void Remove(IMapEntity entity)
        {
            var loggingEntity = (LoggingMapEntity)entity;
            _logger.Verbose("[{ClassName}, {Index}] Removing entity", entity.KeyValues[KeyValueUtilities.ClassName], IndexOf(entity));
            _map.Remove(loggingEntity.Entity);
        }

        public override void Serialize(Stream stream)
        {
            _map.Serialize(stream);
        }
    }
}
