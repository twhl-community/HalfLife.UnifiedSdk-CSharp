using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Tools;
using System.Collections.Generic;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile
{
    /// <summary>Base class for compiled maps.</summary>
    internal abstract class BSPMapBase : Map
    {
        /// <summary>The entities lump object containing this map's entity data.</summary>
        protected readonly Sledge.Formats.Bsp.Lumps.Entities _entitiesLump;

        /// <summary>Creates a new map with the given file name and entities lump.</summary>
        /// <exception cref="System.ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="entitiesLump"/> contains no entities.
        /// -or- <paramref name="entitiesLump"/>'s first entity is not <c>worldspawn</c>.
        /// -or- <paramref name="entitiesLump"/> has more than one <c>worldspawn</c>.
        /// </exception>
        protected BSPMapBase(string fileName, Sledge.Formats.Bsp.Lumps.Entities entitiesLump)
            : base(fileName, MapContentType.Compiled)
        {
            _entitiesLump = entitiesLump;
        }

        protected override EntityList CreateEntities()
        {
            return new BSPEntityList(this);
        }

        internal IEnumerable<Entity> GetEntities(EntityList entityList)
        {
            return _entitiesLump
                .Select(e => new BSPEntity(entityList, e, e.ClassName == KeyValueUtilities.WorldspawnClassName))
                .ToList();
        }

        internal Entity CreateNewEntity(string className)
        {
            var entity = new BSPEntity(Entities, new Sledge.Formats.Bsp.Objects.Entity
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
