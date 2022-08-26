using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools;
using Serilog;
using System.IO;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    internal sealed class LoggingMap : Map
    {
        private readonly ILogger _logger;

        public LoggingMap(MapData mapData, ILogger logger)
            : base(mapData, false)
        {
            _logger = logger;
            Entities = new EntityList(this, mapData.GetEntities().Select(e => new LoggingMapEntity(e, this, _logger)));
        }

        internal override IMapEntity CreateNewEntity(string className)
        {
            _logger.Verbose("Creating entity of class {ClassName}", className);
            return new LoggingMapEntity(base.CreateNewEntity(className), this, _logger);
        }

        internal override int IndexOf(IMapEntity entity)
        {
            var loggingEntity = (LoggingMapEntity)entity;
            return base.IndexOf(loggingEntity.Entity);
        }

        internal override void Add(IMapEntity entity)
        {
            var loggingEntity = (LoggingMapEntity)entity;
            base.Add(loggingEntity.Entity);
            _logger.Verbose("[{ClassName}, {Index}] Adding entity", entity.KeyValues[KeyValueUtilities.ClassName], IndexOf(entity));
        }

        internal override void Remove(IMapEntity entity)
        {
            var loggingEntity = (LoggingMapEntity)entity;
            _logger.Verbose("[{ClassName}, {Index}] Removing entity", entity.KeyValues[KeyValueUtilities.ClassName], IndexOf(entity));
            base.Remove(loggingEntity.Entity);
        }
    }
}
