using HalfLife.UnifiedSdk.Utilities.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Entities
{
    /// <summary>
    /// Encapsulates an entity and provides all of its keyvalues as two sets of lists: a read-only original and a mutable current set.
    /// </summary>
    public abstract class Entity : IList<KeyValuePair<string, string>>
    {
        private readonly EntityList _entityList;

        private readonly List<KeyValuePair<string, string>> _currentKeyValues;

        /// <summary>The keyvalues that the entity had stored in the map.</summary>
        public ImmutableList<KeyValuePair<string, string>> OriginalKeyValues { get; }

        /// <summary>Number of keyvalues in this entity, including the class name.</summary>
        public int Count => _currentKeyValues.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>Gets or sets the keyvalue with the given <paramref name="key"/>.</summary>
        public string this[string key]
        {
            get => GetStringOrNull(key) ?? throw new KeyNotFoundException(nameof(key));
            set => SetString(key, value);
        }

        /// <inheritdoc/>
        public KeyValuePair<string, string> this[int index]
        {
            get => _currentKeyValues[index];
            set => Insert(index, value);
        }

        /// <summary>The entity's class name.</summary>
        public string ClassName
        {
            get => this[KeyValueUtilities.ClassName];
            set => this[KeyValueUtilities.ClassName] = value;
        }

        /// <summary>Whether this entity is the worldspawn entity.</summary>
        public abstract bool IsWorldspawn { get; }

        /// <summary>
        /// Creates a new entity with the given keyvalues that is part of the given entity list.
        /// </summary>
        /// <exception cref="ArgumentException">If the classname is missing or contains only whitespace.</exception>
        protected Entity(EntityList entityList, ImmutableList<KeyValuePair<string, string>> keyValues)
        {
            _entityList = entityList;

            OriginalKeyValues = keyValues;

            _currentKeyValues = OriginalKeyValues.ToList();

            // Don't validate classnames here; some maps have bad classnames and we need to be able to load them.
        }

        /// <summary>Returns whether any keyvalue contains <paramref name="key"/>.</summary>
        public bool ContainsKey(string key) => _currentKeyValues.Find(kv => kv.Key == key).Key is not null;

        /// <summary>Returns whether any keyvalue contains <paramref name="value"/>.</summary>
        public bool ContainsValue(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return _currentKeyValues.Find(kv => kv.Value == value).Key is not null;
        }

        /// <summary>Gets the first occurrence of <paramref name="key"/>.</summary>
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            value = GetStringOrNull(key);
            return value is not null;
        }

        /// <summary>Gets the value of the given <paramref name="key"/> if it exists, <see langword="null"/> otherwise.</summary>
        public string? GetStringOrNull(string key)
        {
            var result = _currentKeyValues.Find(kv => kv.Key == key);

            if (result.Key is not null)
            {
                return result.Value;
            }

            return null;
        }

        /// <summary>Gets the value of the given <paramref name="key"/> if its exists, <paramref name="defaultValue"/> otherwise.</summary>
        public string GetString(string key, string defaultValue = "")
        {
            return GetStringOrNull(key) ?? defaultValue;
        }

        /// <summary>Sets <paramref name="key"/> to <paramref name="value"/>.</summary>
        /// <param name="key">Key to add or set. Must contain at least one non-whitespace character.</param>
        /// <param name="value">Value to set.</param>
        /// <remarks>
        /// If this entity is <c>worldspawn</c> then <paramref name="key"/> may not be <c>classname</c>.
        /// If <paramref name="key"/> is <c>classname</c>
        /// and this entity not is <c>worldspawn</c> then <paramref name="value"/> may not be <c>worldspawn</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> is invalid.
        /// -or- <paramref name="key"/> is <c>classname</c> and <paramref name="value"/> is not a valid classname,
        /// the current entity is <c>worldspawn</c>
        /// or the current entity is not <c>worldspawn</c> and <paramref name="value"/> is <c>worldspawn</c>.
        /// </exception>
        public void SetString(string key, string value)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);

            ValidateKeyValue(key, value);

            // Replace first occurrence; ignore duplicates.
            int index = _currentKeyValues.FindIndex(kv => kv.Key == key);

            bool overwrite = index != -1;

            if (index == -1)
            {
                index = _currentKeyValues.Count;
            }

            InternalInsert(index, new(key, value), overwrite);
        }

        private void ValidateKeyValue(string key, string value)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key must be valid", nameof(key));
            }

            if (key == KeyValueUtilities.ClassName)
            {
                //Classnames are validated to prevent users from making mistakes that can cause problems at runtime.

                KeyValueUtilities.ValidateClassName(value);

                //Only one worldspawn entity can exist in a map, and it must be the first entity in the map.
                if (IsWorldspawn)
                {
                    throw new ArgumentException("Cannot change classname of worldspawn", nameof(value));
                }

                if (value == KeyValueUtilities.WorldspawnClassName)
                {
                    throw new ArgumentException($"Cannot change classname of entity with class {ClassName} to worldspawn", nameof(value));
                }
            }
        }

        private void InternalInsert(int index, KeyValuePair<string, string> item, bool overwrite)
        {
            string? previousValue = index != _currentKeyValues.Count ? _currentKeyValues[index].Value : null;

            if (overwrite)
            {
                _currentKeyValues[index] = item;
            }
            else
            {
                _currentKeyValues.Insert(index, item);
            }

            SetKeyValue(index, item.Key, item.Value, overwrite);

            _entityList.ChangedKeyValue(this, item.Key, previousValue, item.Value);
        }

        /// <summary>Removes all occurrences of <paramref name="key"/> from the entity.</summary>
        /// <exception cref="ArgumentException"><paramref name="key"/> is <c>classname</c></exception>
        public bool Remove(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (key == KeyValueUtilities.ClassName)
            {
                throw new ArgumentException("Cannot remove classname key", nameof(key));
            }

            _entityList.InternalRemovingKeyValue(this, key);

            bool removedAny = false;

            for (int i = _currentKeyValues.Count - 1; i >= 0; --i)
            {
                var kv = _currentKeyValues[i];

                if (kv.Key == key)
                {
                    removedAny = true;
                    RemoveKeyValue(i, key);
                    _currentKeyValues.RemoveAt(i);
                }
            }

            return removedAny;
        }

        /// <summary>Removes all keyvalues except for the class name.</summary>
        public void Clear()
        {
            var className = ClassName;
            _currentKeyValues.Clear();
            _currentKeyValues.Add(new(KeyValueUtilities.ClassName, className));

            _entityList.InternalRemovingAllKeyValues(this);

            RemoveAllKeyValues();
        }

        /// <summary>Returns an enumerator that iterates through the <see cref="Entity"/>.</summary>
        public List<KeyValuePair<string, string>>.Enumerator GetEnumerator() => _currentKeyValues.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc cref="SetString(string, string)"/>
        /// <seealso cref="SetString(string, string)"/>
        public void Add(string key, string value) => SetString(key, value);

        /// <inheritdoc/>
        public override string ToString()
        {
            var result = ClassName + ":" + _entityList.IndexOf(this);

            var targetName = this.GetTargetName();

            if (!string.IsNullOrEmpty(targetName))
            {
                result = result + ":" + targetName;
            }

            return result;
        }

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item) => SetString(item.Key, item.Value);

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) => _currentKeyValues.Contains(item);

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _currentKeyValues.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            var index = _currentKeyValues.FindIndex(kv => kv.Key == item.Key && kv.Value == item.Value);

            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);

            return true;
        }

        /// <inheritdoc/>
        public int IndexOf(KeyValuePair<string, string> item)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(item.Key);
            ArgumentNullException.ThrowIfNull(item.Value);

            return _currentKeyValues.IndexOf(item);
        }

        /// <inheritdoc cref="IndexOf(KeyValuePair{string, string})"/>
        public int IndexOf(string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return _currentKeyValues.FindIndex(kv => kv.Key == key);
        }

        /// <inheritdoc/>
        public void Insert(int index, KeyValuePair<string, string> item)
        {
            if (index < 0 || index > _currentKeyValues.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index out of range");
            }

            ArgumentNullException.ThrowIfNull(item);

            ValidateKeyValue(item.Key, item.Value);
            InternalInsert(index, item, false);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _currentKeyValues.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index out of range");
            }

            var key = _currentKeyValues[index].Key;

            if (key == KeyValueUtilities.ClassName)
            {
                throw new ArgumentException("Cannot remove classname key", nameof(index));
            }

            _entityList.InternalRemovingKeyValue(this, key);

            RemoveKeyValue(index, key);
            _currentKeyValues.RemoveAt(index);
        }

        /// <summary>Sets the keyvalue in the underlying entity object.</summary>
        protected abstract void SetKeyValue(int index, string key, string value, bool overwrite);

        /// <summary>Removes the keyvalue from the underlying entity object.</summary>
        protected abstract void RemoveKeyValue(int index, string key);

        /// <summary>Removes all keyvalues except for the classname.</summary>
        protected abstract void RemoveAllKeyValues();
    }
}
