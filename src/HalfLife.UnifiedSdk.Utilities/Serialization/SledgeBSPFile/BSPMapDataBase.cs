using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Tools;
using System.Collections.Generic;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile
{
    /// <summary>Base class for compiled maps.</summary>
    internal abstract class BSPMapDataBase : MapData
    {
        /// <summary>The entities lump object containing this map's entity data.</summary>
        protected readonly Sledge.Formats.Bsp.Lumps.Entities _entitiesLump;

        private EntityList? _entityList;

        /// <summary>Creates a new map with the given file name and entities lump.</summary>
        /// <exception cref="System.ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="entitiesLump"/> contains no entities.
        /// -or- <paramref name="entitiesLump"/>'s first entity is not <c>worldspawn</c>.
        /// -or- <paramref name="entitiesLump"/> has more than one <c>worldspawn</c>.
        /// </exception>
        protected BSPMapDataBase(string fileName, Sledge.Formats.Bsp.Lumps.Entities entitiesLump)
            : base(fileName, MapContentType.Compiled)
        {
            _entitiesLump = entitiesLump;
        }

        public override EntityList CreateEntities()
        {
            return _entityList ??= new BSPEntityList(this);
        }

        internal IEnumerable<Entity> GetEntities(EntityList entityList)
        {
            return _entitiesLump
                .Select(e => new BSPEntity(entityList, e, e.ClassName == KeyValueUtilities.WorldspawnClassName))
                .ToList();
        }

        internal Entity CreateNewEntity(string className)
        {
            var entity = new BSPEntity(_entityList!, new Sledge.Formats.Bsp.Objects.Entity
            {
                ClassName = className
            },
            false);

            _entitiesLump.Add(entity.Entity);

            return entity;
        }

        internal void Remove(Entity entity)
        {
            var bspEntity = (BSPEntity)entity;
            _entitiesLump.Remove(bspEntity.Entity);
        }
    }
}
