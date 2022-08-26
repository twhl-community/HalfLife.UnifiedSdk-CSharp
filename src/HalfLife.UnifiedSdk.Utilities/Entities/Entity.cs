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
    /// Encapsulates an entity and provides all of its keyvalues as two sets of dictionaries: a read-only original and a mutable current set.
    /// </summary>
    public abstract class Entity : IDictionary<string, string>
    {
        private readonly EntityList _entityList;

        private readonly Dictionary<string, string> _currentKeyValues;

        /// <summary>The keyvalues that the entity had stored in the map</summary>
        public ImmutableDictionary<string, string> OriginalKeyValues { get; }

        /// <summary>Number of keyvalues in this entity, including the class name.</summary>
        public int Count => _currentKeyValues.Count;

        /// <inheritdoc/>
        public ICollection<string> Keys => _currentKeyValues.Keys;

        /// <inheritdoc/>
        public ICollection<string> Values => _currentKeyValues.Values;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>Gets or sets the keyvalue with the given <paramref name="key"/>.</summary>
        public string this[string key]
        {
            get => _currentKeyValues[key];
            set => SetString(key, value);
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
        protected Entity(EntityList entityList, ImmutableDictionary<string, string> keyValues)
        {
            _entityList = entityList;

            OriginalKeyValues = keyValues;

            _currentKeyValues = OriginalKeyValues.ToDictionary(kv => kv.Key, kv => kv.Value);

            if (string.IsNullOrWhiteSpace(ClassName))
            {
                throw new ArgumentException("Cannot set classname to null or empty strings", nameof(keyValues));
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key) => _currentKeyValues.ContainsKey(key);

        /// <inheritdoc/>
        public bool ContainsValue(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return _currentKeyValues.ContainsValue(value);
        }

        /// <inheritdoc/>
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value) => _currentKeyValues.TryGetValue(key, out value);

        /// <summary>Gets the value of the given <paramref name="key"/> if it exists, <see langword="null"/> otherwise.</summary>
        public string? GetStringOrNull(string key)
        {
            if (_currentKeyValues.TryGetValue(key, out var value))
            {
                return value;
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

            _currentKeyValues.TryGetValue(key, out var previousValue);

            _currentKeyValues[key] = value;
            SetKeyValue(key, value);

            _entityList.ChangedKeyValue(this, key, previousValue, value);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"><paramref name="key"/> is <c>worldspawn</c></exception>
        public bool Remove(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (key == KeyValueUtilities.ClassName)
            {
                throw new ArgumentException("Cannot remove classname key", nameof(key));
            }

            _entityList.InternalRemovingKeyValue(this, key);

            RemoveKeyValue(key);
            return _currentKeyValues.Remove(key);
        }

        /// <summary>Removes all keyvalues except for the class name.</summary>
        public void Clear()
        {
            var className = ClassName;
            _currentKeyValues.Clear();
            _currentKeyValues.Add(KeyValueUtilities.ClassName, className);

            _entityList.InternalRemovingAllKeyValues(this);

            RemoveAllKeyValues();
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _currentKeyValues.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc cref="SetString(string, string)"/>
        /// <seealso cref="SetString(string, string)"/>
        public void Add(string key, string value) => SetString(key, value);

        /// <inheritdoc/>
        public bool TryAdd(string key, string value)
        {
            if (_currentKeyValues.ContainsKey(key))
            {
                return false;
            }

            SetString(key, value);

            return true;
        }

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
            ((IDictionary<string, string>)_currentKeyValues).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            //ICollection.Remove requires both key and value to match, so do this for consistency.
            if (!_currentKeyValues.Contains(item))
            {
                return false;
            }

            return Remove(item.Key);
        }

        /// <summary>Sets the keyvalue in the underlying entity object.</summary>
        protected abstract void SetKeyValue(string key, string value);

        /// <summary>Removes the keyvalue from the underlying entity object.</summary>
        protected abstract void RemoveKeyValue(string key);

        /// <summary>Removes all keyvalues except for the classname.</summary>
        protected abstract void RemoveAllKeyValues();
    }
}
