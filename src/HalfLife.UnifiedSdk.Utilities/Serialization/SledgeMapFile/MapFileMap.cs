using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Maps;
using Sledge.Formats.Map.Formats;
using Sledge.Formats.Map.Objects;
using System.Collections.Generic;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile
{
    internal sealed class MapFileMap : Map
    {
        private readonly MapFile _mapFile;
        private readonly IMapFormat _format;
        private readonly string _styleHint;

        private readonly List<IMapEntity> _entities;

        public override EntityList Entities { get; }

        internal MapFileMap(string fileName, MapFile mapFile, IMapFormat format, string styleHint)
            : base(fileName, MapContentType.Source)
        {
            _mapFile = mapFile;
            _format = format;
            _styleHint = styleHint;

            _entities = new();

            GetEntities(_entities, _mapFile.Worldspawn);

            Entities = new(this, _entities);
        }

        internal override IMapEntity CreateNewEntity(string className)
        {
            return new MapFileEntity(new Sledge.Formats.Map.Objects.Entity()
            {
                ClassName = className
            },
            false);
        }

        internal override void Add(IMapEntity entity)
        {
            var mapEntity = (MapFileEntity)entity;

            _entities.Add(entity);

            _mapFile.Worldspawn.Children.Add(mapEntity.Entity);
        }

        internal override void Remove(IMapEntity entity)
        {
            var mapEntity = (MapFileEntity)entity;

            _entities.Remove(entity);

            RemoveEntity(_mapFile.Worldspawn, mapEntity.Entity);
        }

        public override void Serialize(Stream stream)
        {
            _format.Write(stream, _mapFile, _styleHint);
        }

        private static void GetEntities(List<IMapEntity> entities, MapObject obj)
        {
            if (obj is Sledge.Formats.Map.Objects.Entity entity)
            {
                entities.Add(new MapFileEntity(entity, entity is Worldspawn));
            }

            switch (obj)
            {
                case Worldspawn:
                case Group:
                    foreach (var child in obj.Children)
                    {
                        GetEntities(entities, child);
                    }

                    break;
            }
        }

        /// <summary>
        /// This can never remove worldspawn
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="entity"></param>
        private static bool RemoveEntity(MapObject obj, Sledge.Formats.Map.Objects.Entity entity)
        {
            foreach (var child in obj.Children)
            {
                if (child == entity)
                {
                    obj.Children.Remove(entity);
                    return true;
                }

                if (child is Group && RemoveEntity(child, entity))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
