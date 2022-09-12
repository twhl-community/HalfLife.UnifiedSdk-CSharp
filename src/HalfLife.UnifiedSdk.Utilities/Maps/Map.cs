using HalfLife.UnifiedSdk.Utilities.Entities;
using System;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    /// <summary>Provides access to map data.</summary>
    public abstract class Map
    {
        /// <summary>File name of this map.</summary>
        public string FileName { get; }

        /// <summary>
        /// Base name of the map, used in trigger_changelevel and the changelevel command among other things.
        /// </summary>
        public string BaseName => Path.GetFileNameWithoutExtension(FileName);

        /// <summary>Which content type the map data is in.</summary>
        /// <seealso cref="MapContentType"/>
        public MapContentType ContentType { get; }

        /// <summary>Whether this map is a compiled or in a source content type.</summary>
        /// <seealso cref="MapContentType"/>
        public bool IsCompiled => ContentType == MapContentType.Compiled;

        private EntityList? _entityList;

        /// <summary>List of entities in the map.</summary>
        public EntityList Entities => _entityList ??= CreateEntities();

        /// <summary>Creates a map data object.</summary>
        /// <exception cref="ArgumentNullException">If <paramref name="fileName"/> is <see langword="null"/>.</exception>
        protected Map(string fileName, MapContentType contentType)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            ContentType = contentType;
        }

        /// <summary>Creates an entity list from this map's data. The entity list can modify the map's entities.</summary>
        protected abstract EntityList CreateEntities();

        /// <summary>Serializes this map to the given stream.</summary>
        public abstract void Serialize(Stream stream);

        /// <inheritdoc/>
        public override string ToString() => BaseName;
    }
}
