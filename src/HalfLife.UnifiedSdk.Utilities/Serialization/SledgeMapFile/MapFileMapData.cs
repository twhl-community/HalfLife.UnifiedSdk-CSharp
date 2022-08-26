using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Maps;
using Sledge.Formats.Map.Formats;
using System.Collections.Generic;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile
{
    internal sealed class MapFileMapData : MapData
    {
        private readonly Sledge.Formats.Map.Objects.MapFile _mapFile;
        private readonly IMapFormat _format;
        private readonly string _styleHint;

        internal MapFileMapData(string fileName, Sledge.Formats.Map.Objects.MapFile mapFile, IMapFormat format, string styleHint)
            : base(fileName, MapContentType.Source)
        {
            _mapFile = mapFile;
            _format = format;
            _styleHint = styleHint;
        }

        public override EntityList CreateEntities()
        {
            return new MapFileEntityList(this);
        }

        public IEnumerable<Entity> GetEntities()
        {
            List<Entity> entities = new();

            GetEntities(entities, _mapFile.Worldspawn);

            return entities;
        }

        public Entity CreateNewEntity(string className)
        {
            var entity = new MapFileEntity(new Sledge.Formats.Map.Objects.Entity()
            {
                ClassName = className
            },
            false);

            _mapFile.Worldspawn.Children.Add(entity.Entity);

            return entity;
        }

        public void Remove(Entity entity)
        {
            var mapEntity = (MapFileEntity)entity;
            RemoveEntity(_mapFile.Worldspawn, mapEntity.Entity);
        }

        public override void Serialize(Stream stream)
        {
            _format.Write(stream, _mapFile, _styleHint);
        }

        private static void GetEntities(List<Entity> entities, Sledge.Formats.Map.Objects.MapObject obj)
        {
            if (obj is Sledge.Formats.Map.Objects.Entity entity)
            {
                entities.Add(new MapFileEntity(entity, entity is Sledge.Formats.Map.Objects.Worldspawn));
            }

            switch (obj)
            {
                case Sledge.Formats.Map.Objects.Worldspawn:
                case Sledge.Formats.Map.Objects.Group:
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
        private static bool RemoveEntity(Sledge.Formats.Map.Objects.MapObject obj, Sledge.Formats.Map.Objects.Entity entity)
        {
            foreach (var child in obj.Children)
            {
                if (child == entity)
                {
                    obj.Children.Remove(entity);
                    return true;
                }

                if (child is Sledge.Formats.Map.Objects.Group && RemoveEntity(child, entity))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
