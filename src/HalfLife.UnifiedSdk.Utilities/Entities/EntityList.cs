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
    public abstract class EntityList : IReadOnlyList<Entity>, ICollection<Entity>
    {
        /// <summary>All entities currently in the map.</summary>
        private readonly List<Entity> _entities;

        /// <summary>Total number of entities in the list, including <c>worldspawn</c>.</summary>
        public int Count => _entities.Count;

        /// <inheritdoc/>
        public Entity this[int index] => _entities[index];

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>Gets the worldspawn entity.</summary>
        public Entity Worldspawn => _entities[0];

        /// <summary>Raised when an entity is created.</summary>
        public event EventHandler<EntityEventArgs>? EntityCreated;

        /// <summary>Raised when an entity is about to be removed.</summary>
        public event EventHandler<EntityEventArgs>? RemovingEntity;

        /// <summary>Raised when a keyvalue is changed in the given entity.</summary>
        public event EventHandler<EntityKeyValueChangedEventArgs>? KeyValueChanged;

        /// <summary>Raised when a keyvalue is about to be removed from the given entity.</summary>
        public event EventHandler<EntityKeyValueRemovingEventArgs>? RemovingKeyValue;

        /// <summary>Raised when all keyvalues are about to be removed from the given entity.</summary>
        public event EventHandler<EntityEventArgs>? RemovingAllKeyValues;

        internal EntityList(Func<EntityList, IEnumerable<Entity>> getEntitiesCallback)
        {
            _entities = getEntitiesCallback(this).ToList();

            if (_entities.Count == 0)
            {
                throw new ArgumentException("Entity list must contain at least one entity", nameof(getEntitiesCallback));
            }

            if (_entities[0].ClassName != KeyValueUtilities.WorldspawnClassName)
            {
                throw new ArgumentException("First entity in the entity list must be worldspawn", nameof(getEntitiesCallback));
            }

            int numberOfWorldspawn = _entities.Count(e => e.ClassName == KeyValueUtilities.WorldspawnClassName);

            if (numberOfWorldspawn > 1)
            {
                throw new ArgumentException(
                    $"Too many worldspawn entities in the entity list ({numberOfWorldspawn} found)", nameof(getEntitiesCallback));
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        public int IndexOf(Entity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return _entities.IndexOf(entity);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        public bool Contains(Entity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return _entities.Contains(entity);
        }

        /// <summary>
        /// Creates a new entity with the given class name.
        /// </summary>
        /// <remarks> You cannot create new worldspawn entities. </remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="className"/> is invalid.
        /// -or- <paramref name="className"/> is <c>worldspawn</c>.
        /// </exception>
        public Entity CreateNewEntity(string className)
        {
            KeyValueUtilities.ValidateClassName(className);

            if (className == KeyValueUtilities.WorldspawnClassName)
            {
                throw new ArgumentException("Cannot create new worldspawn entity", nameof(className));
            }

            var entity = CreateNewEntityCore(className);

            _entities.Add(entity);

            EntityCreated?.Invoke(this, new EntityEventArgs(entity));

            return entity;
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
            ArgumentNullException.ThrowIfNull(entity);

            if (entity.IsWorldspawn)
            {
                throw new ArgumentException("Cannot clone worldspawn entities", nameof(entity));
            }

            var newEntity = CreateNewEntity(entity.ClassName);

            newEntity.ReplaceKeyValues(entity);

            return newEntity;
        }

        /// <summary>Not supported. Use <see cref="CreateNewEntity"/> or <see cref="CloneEntity"/> instead.</summary>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        void ICollection<Entity>.Add(Entity entity)
        {
            throw new NotSupportedException(
                $"Manually adding entities to an entity list is not supported. Use {nameof(CreateNewEntity)} or {nameof(CloneEntity)} instead.");
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="entity"/> is worldspawn.</exception>
        public bool Remove(Entity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (entity.IsWorldspawn)
            {
                throw new ArgumentException("Cannot remove worldspawn entity", nameof(entity));
            }

            var index = _entities.IndexOf(entity);

            if (index == -1)
            {
                return false;
            }

            RemoveAtIndex(index);

            return true;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            RemoveAtIndex(index);
        }

        private void RemoveAtIndex(int index)
        {
            var entity = _entities[index];

            if (entity.IsWorldspawn)
            {
                throw new ArgumentException("Cannot remove worldspawn entity", nameof(index));
            }

            RemovingEntity?.Invoke(this, new EntityEventArgs(entity));

            _entities.RemoveAt(index);

            RemoveAtCore(entity, index);
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
        public override string ToString() => $"{Count} Entities";

        /// <inheritdoc/>
        public IEnumerator<Entity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Creates a new entity with the given classname.</summary>
        protected abstract Entity CreateNewEntityCore(string className);

        /// <summary>Removes the given entity from the underlying list.</summary>
        protected abstract void RemoveAtCore(Entity entity, int index);

        internal void ChangedKeyValue(Entity entity, string key, string? previousValue, string currentValue)
        {
            KeyValueChanged?.Invoke(this, new EntityKeyValueChangedEventArgs(entity, key, previousValue, currentValue));
        }

        internal void InternalRemovingKeyValue(Entity entity, string key)
        {
            RemovingKeyValue?.Invoke(this, new EntityKeyValueRemovingEventArgs(entity, key));
        }

        internal void InternalRemovingAllKeyValues(Entity entity)
        {
            RemovingAllKeyValues?.Invoke(this, new EntityEventArgs(entity));
        }
    }
}
