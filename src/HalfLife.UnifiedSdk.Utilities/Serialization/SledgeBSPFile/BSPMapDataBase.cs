﻿using HalfLife.UnifiedSdk.Utilities.Maps;
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

        private readonly List<BSPEntity> _entities;

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

            _entities = _entitiesLump
                .Select(e => new BSPEntity(e, e.ClassName == KeyValueUtilities.WorldspawnClassName))
                .ToList();
        }

        public override IEnumerable<IMapEntity> GetEntities() => _entities;

        public override IMapEntity CreateNewEntity(string className)
        {
            return new BSPEntity(new Sledge.Formats.Bsp.Objects.Entity
            {
                ClassName = className
            },
            false);
        }

        public override int IndexOf(IMapEntity entity)
        {
            return _entities.IndexOf((BSPEntity)entity);
        }

        public override void Add(IMapEntity entity)
        {
            var bspEntity = (BSPEntity)entity;

            _entities.Add(bspEntity);

            _entitiesLump.Add(bspEntity.Entity);
        }

        public override void Remove(IMapEntity entity)
        {
            var bspEntity = (BSPEntity)entity;

            _entities.Remove(bspEntity);

            _entitiesLump.Remove(bspEntity.Entity);
        }
    }
}
