using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Serialization;
using HalfLife.UnifiedSdk.Utilities.Serialization.EntFile;
using HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile;
using HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Provides the set of serializers and helpers to deserialize maps.</summary>
    public static class MapFormats
    {
        /// <summary>The <c>.map</c> file serializer.</summary>
        public static IMapSerializer Map { get; } = new QuakeMapFileSerializer();

        /// <summary>The <c>.rmf</c> file serializer.</summary>
        public static IMapSerializer Rmf { get; } = new WorldcraftRmfMapFileSerializer();

        /// <summary>The <c>.bsp</c> file serializer.</summary>
        public static IMapSerializer Bsp { get; } = new BSPSerializer();

        /// <summary>The <c>.ent</c> file serializer.</summary>
        public static IMapSerializer Ent { get; } = new EntSerializer();

        /// <summary>Dictionary of extension (including period ".") => serializer.</summary>
        public static ImmutableDictionary<string, IMapSerializer> Serializers { get; } = new IMapSerializer[]
        {
            Map,
            Rmf,
            Bsp,
            Ent,
        }.ToImmutableDictionary(s => s.Extension);

        /// <summary>Deserializes a map from a stream.</summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">Stream containing a Half-Life 1 map.</param>
        /// <param name="serializer">Serializer to use to deserialize the map.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> is <see langword="null"/>, contains invalid characters or the extension is not supported.
        /// </exception>
        /// <exception cref="IOException">An IO error occurred during deserialization.</exception>
        public static Map Deserialize(string fileName, Stream stream, IMapSerializer serializer)
        {
            return serializer.Deserialize(fileName, stream);
        }

        /// <summary>Deserializes a map from a file.</summary>
        /// <param name="fileName">Path to the file to deserialize. The file must exist and must be a file containing a Half-Life 1 map.</param>
        /// <param name="serializer">Serializer to use to deserialize the map.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> is <see langword="null"/>, contains invalid characters or the extension is not supported.
        /// </exception>
        /// <exception cref="IOException">An IO error occurred during deserialization.</exception>
        public static Map Deserialize(string fileName, IMapSerializer serializer)
        {
            using var stream = File.OpenRead(fileName);
            return Deserialize(fileName, stream, serializer);
        }

        /// <summary>Deserializes a map from a file.</summary>
        /// <param name="fileName">Path to the file to deserialize. The file must exist and must be a file containing a Half-Life 1 map.</param>
        /// <param name="stream">Stream to deserialize from.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> is <see langword="null"/>, contains invalid characters or the extension is not supported.
        /// </exception>
        /// <exception cref="IOException">An IO error occurred during deserialization.</exception>
        public static Map Deserialize(string fileName, Stream stream)
        {
            var extension = Path.GetExtension(fileName) ??
                throw new ArgumentException("Filename has no extension", nameof(fileName));

            if (!Serializers.TryGetValue(extension, out var serializer))
            {
                throw new ArgumentException($"No serializer for extension {extension}", nameof(fileName));
            }

            return Deserialize(fileName, stream, serializer);
        }

        /// <summary>Deserializes a map from a file.</summary>
        /// <param name="fileName">Path to the file to deserialize. The file must exist and must be a file containing a Half-Life 1 map.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> is <see langword="null"/>, contains invalid characters or the extension is not supported.
        /// </exception>
        /// <exception cref="IOException">An IO error occurred during deserialization.</exception>
        public static Map Deserialize(string fileName)
        {
            using var stream = File.OpenRead(fileName);
            return Deserialize(fileName, stream);
        }

        /// <summary>
        /// Returns an enumerable collection of maps in the given directories using known file extensions.
        /// </summary>
        public static IEnumerable<Map> EnumerateMaps(params string[] directories)
        {
            foreach (var directory in directories)
            {
                foreach (var file in Directory.EnumerateFiles(directory))
                {
                    if (Serializers.ContainsKey(Path.GetExtension(file)))
                    {
                        yield return Deserialize(file);
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerable collection of maps in the given directories matching the given extension.
        /// </summary>
        public static IEnumerable<Map> EnumerateMapsWithExtension(string extension, params string[] directories)
        {
            foreach (var directory in directories)
            {
                foreach (var file in Directory.EnumerateFiles(directory, "*" + extension))
                {
                    yield return Deserialize(file);
                }
            }
        }

        /// <summary>
        /// Creates an empty <c>.ent</c> map.
        /// </summary>
        /// <param name="fileName"></param>
        public static Map CreateEntMap(string fileName)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            var entities = new Sledge.Formats.Bsp.Lumps.Entities
            {
                new Sledge.Formats.Bsp.Objects.Entity
                {
                    ClassName = KeyValueUtilities.WorldspawnClassName
                }
            };

            return new EntMap(fileName, entities);
        }
    }
}
