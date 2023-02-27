using HalfLife.UnifiedSdk.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HalfLife.UnifiedSdk.Utilities.Entities
{
    /// <summary>Extensions for <see cref="EntityList"/>.</summary>
    public static class EntityListExtensions
    {
        /// <summary>Gets a list of all entities except worldspawn.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="entityList"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Entity> WithoutWorldspawn(this EntityList entityList)
        {
            ArgumentNullException.ThrowIfNull(entityList);

            //This is the actual entity list, so the first entity is always worldspawn
            //The API forbids the creation of new worldspawn entities and renaming them is also forbidden, so this isn't an issue here
            return entityList.Skip(1);
        }

        /// <summary>
        /// Renames all classes with the classname <paramref name="oldClassName"/> to <paramref name="newClassName"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityList"/>, <paramref name="oldClassName"/> or <paramref name="newClassName"/> are <see langword="null"/>.
        /// </exception>
        public static void RenameClass(this EntityList entityList, string oldClassName, string newClassName)
        {
            ArgumentNullException.ThrowIfNull(entityList);
            ArgumentNullException.ThrowIfNull(oldClassName);
            ArgumentNullException.ThrowIfNull(newClassName);

            if (oldClassName == newClassName)
            {
                return;
            }

            foreach (var entity in entityList)
            {
                if (entity.ClassName == oldClassName)
                {
                    entity.ClassName = newClassName;
                }
            }
        }

        /// <summary>Removes all entities with the given classname.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="entityList"/> or <paramref name="className"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="className"/> is <c>worldspawn</c>.</exception>
        public static EntityList RemoveAllOfClass(this EntityList entityList, string className)
        {
            ArgumentNullException.ThrowIfNull(entityList);
            ArgumentNullException.ThrowIfNull(className);

            if (className == KeyValueUtilities.WorldspawnClassName)
            {
                throw new ArgumentException("Cannot remove worldspawn", nameof(className));
            }

            for (int i = 1; i < entityList.Count;)
            {
                var entity = entityList[i];

                if (entity.ClassName == className)
                {
                    entityList.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }

            return entityList;
        }

        /// <summary>Removes all entities that match the given predicate.</summary>
        /// <returns><paramref name="entityList"/>.</returns>
        /// <remarks><c>worldspawn</c> is not checked against <paramref name="predicate"/>.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="entityList"/> or <paramref name="predicate"/> are <see langword="null"/>.</exception>
        public static EntityList RemoveAll(this EntityList entityList, Predicate<Entity> predicate)
        {
            ArgumentNullException.ThrowIfNull(entityList);
            ArgumentNullException.ThrowIfNull(predicate);

            for (int i = 1; i < entityList.Count;)
            {
                var entity = entityList[i];

                if (predicate(entity))
                {
                    entityList.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }

            return entityList;
        }

        /// <summary>
        /// Invokes <paramref name="callback"/> on every entity with classname <paramref name="className"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityList"/>, <paramref name="className"/> or <paramref name="callback"/> are <see langword="null"/>.
        /// </exception>
        public static void ForEachClass(this EntityList entityList, string className, Action<Entity> callback)
        {
            ArgumentNullException.ThrowIfNull(entityList);
            ArgumentNullException.ThrowIfNull(className);
            ArgumentNullException.ThrowIfNull(callback);

            foreach (var entity in entityList.ToList())
            {
                if (entity.ClassName == className)
                {
                    callback(entity);
                }
            }
        }

        /// <summary>
        /// Replaces this entity list with the contents of another.
        /// This is how Ripent works.
        /// This will only work properly if the entity list is a <c>.bsp</c> file
        /// and the other list is an <c>.ent</c> file made for this map.
        /// </summary>
        /// <returns><paramref name="entityList"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entityList"/> or <paramref name="other"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="entityList"/> and <paramref name="other"/> have differing content types.
        /// </exception>
        public static EntityList ReplaceWith(this EntityList entityList, EntityList other)
        {
            ArgumentNullException.ThrowIfNull(entityList);
            ArgumentNullException.ThrowIfNull(other);

            if (entityList == other)
            {
                return entityList;
            }

            entityList.Clear();

            entityList.Worldspawn.ReplaceWith(other.Worldspawn);

            for (int i = 1; i < other.Count; ++i)
            {
                var source = other[i];

                entityList.CloneEntity(source);
            }

            return entityList;
        }

        /// <summary>
        /// Generates a unique targetname from a base name.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityList"/> or <paramref name="baseName"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="baseName"/> is empty or contains only whitespace
        /// -or- A unique targetname could not be generated for <paramref name="baseName"/>.</exception>
        public static string GenerateUniqueTargetName(this EntityList entityList, string baseName)
        {
            ArgumentNullException.ThrowIfNull(entityList);
            ArgumentNullException.ThrowIfNull(baseName);

            if (string.IsNullOrWhiteSpace(baseName))
            {
                throw new ArgumentException("Base name must contain valid characters", nameof(baseName));
            }

            var builder = new StringBuilder();
            builder.Append(baseName);

            var baseLength = builder.Length;

            for (int number = 0; number != int.MaxValue; ++number)
            {
                builder.Length = baseLength;
                builder.Append(number);

                if (!entityList.Any(e => builder.Equals(e.GetTargetName().AsSpan())))
                {
                    return builder.ToString();
                }
            }

            //Unlikely to happen unless a map has way more entities than the engine supports.
            throw new ArgumentException($"Could not generate unique targetname for \"{baseName}\"", nameof(baseName));
        }

        /// <summary>
        /// Finds the first entity with the given targetname or <see langword="null"/> if no entity with that targetname exists.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entityList"/> or <paramref name="targetName"/> are <see langword="null"/>.
        /// </exception>
        public static Entity? Find(this EntityList entityList, string targetName)
        {
            ArgumentNullException.ThrowIfNull(entityList);
            ArgumentNullException.ThrowIfNull(targetName);

            foreach (var entity in entityList)
            {
                if (entity.GetTargetName() == targetName)
                {
                    return entity;
                }
            }

            return null;
        }
    }
}
