using HalfLife.UnifiedSdk.Utilities.Maps;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Serialization
{
    /// <summary>Represents a means of deserializing maps.</summary>
    public interface IMapSerializer
    {
        /// <summary>Which file extension this serializer applies to (including the period ".").</summary>
        string Extension { get; }

        /// <summary>Deserializes a map into a <see cref="Map"/> object.</summary>
        /// <param name="fileName">Name of the file being deserialized.</param>
        /// <param name="stream">Stream to deserialize the map from.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="fileName"/> is <see langword="null"/>, contains invalid characters or the extension is not supported.
        /// -or- <paramref name="stream"/> contains an empty map.
        /// -or- <paramref name="stream"/> contains a map whose first entity is not <c>worldspawn</c>.
        /// -or- <paramref name="stream"/> contains a map with more than one <c>worldspawn</c>.
        /// </exception>
        /// <exception cref="IOException">An IO error occurred during deserialization.</exception>
        Map Deserialize(string fileName, Stream stream);
    }
}
