using HalfLife.UnifiedSdk.Utilities.Entities;
using System;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    /// <summary>
    /// Contains data about a map loaded from a stream.
    /// </summary>
    public abstract class MapData
    {
        /// <summary>File name of this map.</summary>
        public string FileName { get; }

        /// <summary>Which content type the map data is in.</summary>
        /// <seealso cref="MapContentType"/>
        public MapContentType ContentType { get; }

        /// <summary>Creates a map data object.</summary>
        /// <exception cref="ArgumentNullException">If <paramref name="fileName"/> is <see langword="null"/>.</exception>
        protected MapData(string fileName, MapContentType contentType)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            ContentType = contentType;
        }

        /// <summary>Creates an entity list from this map's data. The entity list can modify the map's entities.</summary>
        public abstract EntityList CreateEntities();

        /// <summary>Serializes this map to the given stream.</summary>
        public abstract void Serialize(Stream stream);
    }
}
