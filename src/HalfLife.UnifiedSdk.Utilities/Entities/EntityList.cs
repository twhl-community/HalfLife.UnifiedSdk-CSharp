using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Entities
{
    /// <summary>
    /// Manages the list of entities.
    /// Implements most of the <see cref="IList{T}"/> interface, but not all of it due to limitations of the underlying data structures.
    /// </summary>
    public sealed class EntityList : IReadOnlyList<Entity>, ICollection<Entity>
    {
        /// <summary>All entities currently in the map.</summary>
        private readonly List<Entity> _entities;

        /// <summary>The map that this entity list belongs to.</summary>
        public Map Map { get; }

        /// <summary>Total number of entities in the list, including <c>worldspawn</c>.</summary>
        public int Count => _entities.Count;

        /// <inheritdoc/>
        public Entity this[int index] => _entities[index];

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>Gets the worldspawn entity.</summary>
        public Entity Worldspawn => _entities[0];

        internal EntityList(Map map, IEnumerable<IEntity> entities)
        {
            Map = map;

            _entities = entities
                .Select(e => new Entity(this, e))
                .ToList();

            if (_entities.Count == 0)
            {
                throw new ArgumentException("Entity list must contain at least one entity", nameof(entities));
            }

            if (_entities[0].ClassName != KeyValueUtilities.WorldspawnClassName)
            {
                throw new ArgumentException("First entity in the entity list must be worldspawn", nameof(entities));
            }

            int numberOfWorldspawn = _entities.Count(e => e.ClassName == KeyValueUtilities.WorldspawnClassName);

            if (numberOfWorldspawn > 1)
            {
                throw new ArgumentException($"Too many worldspawn entities in the entity list ({numberOfWorldspawn} found)", nameof(entities));
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        public int IndexOf(Entity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return _entities.IndexOf(entity);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        public bool Contains(Entity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return _entities.Contains(entity);
        }

        /// <summary>
        /// Creates a new entity with the given class name.
        /// The entity will not be added to the entity list.
        /// </summary>
        /// <remarks> You cannot create new worldspawn entities. </remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="className"/> is invalid.
        /// -or- <paramref name="className"/> is <c>worldspawn</c>.
        /// </exception>
        /// <seealso cref="Add(Entity)"/>
        public Entity CreateNewEntity(string className)
        {
            KeyValueUtilities.ValidateClassName(className);

            if (className == KeyValueUtilities.WorldspawnClassName)
            {
                throw new ArgumentException("Cannot create new worldspawn entity", nameof(className));
            }

            return new Entity(this, Map.CreateNewEntity(className));
        }

        /// <summary>
        /// Creates a clone of the given entity.
        /// Does not clone any data associated with the entity such as brush data. Only keyvalues are cloned.
        /// </summary>
        /// <remarks> You cannot clone <c>worldspawn</c>.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="entity"/>is <c>worldspawn</c>.</exception>
        public Entity CloneEntity(Entity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.IsWorldspawn)
            {
                throw new ArgumentException("Cannot clone worldspawn entities", nameof(entity));
            }

            var newEntity = new Entity(this, Map.CreateNewEntity(entity.ClassName));

            newEntity.ReplaceKeyValues(entity);

            return newEntity;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="entity"/>belongs to another entity list.
        /// -or- <paramref name="entity"/> is worldspawn.
        /// -or- <paramref name="entity"/> is already in this entity list.
        /// </exception>
        public void Add(Entity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.Entities != this)
            {
                throw new ArgumentException("Cannot add entities made by another entity list", nameof(entity));
            }

            if (entity.IsWorldspawn)
            {
                throw new ArgumentException("Cannot add a second worldspawn entity", nameof(entity));
            }

            if (_entities.Contains(entity))
            {
                throw new ArgumentException($"Trying to add entity {entity} that is already in the list", nameof(entity));
            }

            _entities.Add(entity);
            Map.Add(entity._entity);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="entity"/> is worldspawn.</exception>
        public bool Remove(Entity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.IsWorldspawn)
            {
                throw new ArgumentException("Cannot remove worldspawn entity", nameof(entity));
            }

            var index = _entities.IndexOf(entity);

            if (index == -1)
            {
                return false;
            }

            RemoveAtCore(index);

            return true;
        }

        private void RemoveAtCore(int index)
        {
            var entity = _entities[index];

            if (entity.IsWorldspawn)
            {
                throw new ArgumentException("Cannot remove worldspawn entity", nameof(index));
            }

            _entities.RemoveAt(index);

            Map.Remove(entity._entity);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            RemoveAtCore(index);
        }

        /// <summary>Removes all entities except worldspawn, and removes all keyvalues from worldspawn.</summary>
        public void Clear()
        {
            for (int i = Count - 1; i > 0; --i)
            {
                RemoveAt(i);
            }

            Worldspawn.Clear();
        }

        /// <inheritdoc/>
        public void CopyTo(Entity[] array, int arrayIndex)
        {
            _entities.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<Entity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
