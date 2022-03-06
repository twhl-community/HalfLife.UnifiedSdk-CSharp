using HalfLife.UnifiedSdk.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Entities
{
    /// <summary>Extensions for <see cref="IEnumerable{T}"/> of <see cref="Entity"/>.</summary>
    public static class EnumerableEntityExtensions
    {
        /// <summary>Returns an enumerable collection of entities whose classname matches the given name.</summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityList"/> or <paramref name="className"/> are <see langword="null"/>.
        /// </exception>
        public static IEnumerable<Entity> OfClass(this IEnumerable<Entity> entityList, string className)
        {
            if (entityList is null)
            {
                throw new ArgumentNullException(nameof(entityList));
            }

            if (className is null)
            {
                throw new ArgumentNullException(nameof(className));
            }

            return entityList.Where(e => e.ClassName == className);
        }

        /// <summary>Finds all entities that have the given key and value</summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityList"/>, <paramref name="key"/> or <paramref name="value"/> are <see langword="null"/>.
        /// </exception>
        public static IEnumerable<Entity> WhereString(this IEnumerable<Entity> entityList, string key, string value)
        {
            if (entityList is null)
            {
                throw new ArgumentNullException(nameof(entityList));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return entityList.Where(e => e.HasKeyValueCore(key, value));
        }

        /// <summary>Finds all entities that have the given targetname.</summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityList"/> or <paramref name="targetName"/> are <see langword="null"/>.
        /// </exception>
        public static IEnumerable<Entity> WhereTargetName(this IEnumerable<Entity> entityList, string targetName)
        {
            return entityList.WhereString(KeyValueUtilities.TargetName, targetName);
        }

        /// <summary>Finds all entities that have the given target.</summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityList"/> or <paramref name="target"/> are <see langword="null"/>.
        /// </exception>
        public static IEnumerable<Entity> WhereTarget(this IEnumerable<Entity> entityList, string target)
        {
            return entityList.WhereString(KeyValueUtilities.Target, target);
        }

        /// <summary>Gets an enumerable collection of all entities except worldspawn.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="entityList"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Entity> WithoutWorldspawn(this IEnumerable<Entity> entityList)
        {
            if (entityList is null)
            {
                throw new ArgumentNullException(nameof(entityList));
            }

            //Don't assume worldspawn is the first entity (in case of lists that have been reorganized).
            //There could be multiple instances of it in the list.
            return entityList.Where(e => !e.IsWorldspawn);
        }
    }
}
