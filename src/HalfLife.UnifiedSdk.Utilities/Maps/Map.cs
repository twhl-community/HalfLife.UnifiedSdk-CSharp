using HalfLife.UnifiedSdk.Utilities.Entities;
using Serilog;
using System;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    /// <summary>Provides access to map data.</summary>
    public abstract class Map
    {
        private protected readonly MapData _mapData;

        /// <summary>File name of this map.</summary>
        public string FileName => _mapData.FileName;

        /// <summary>
        /// Base name of the map, used in trigger_changelevel and the changelevel command among other things.
        /// </summary>
        public string BaseName => Path.GetFileNameWithoutExtension(FileName);

        /// <summary>Which content type the map data is in.</summary>
        /// <seealso cref="MapContentType"/>
        public MapContentType ContentType => _mapData.ContentType;

        /// <summary>Whether this map is a compiled or in a source content type.</summary>
        /// <seealso cref="MapContentType"/>
        public bool IsCompiled => ContentType == MapContentType.Compiled;

        /// <summary>List of entities in the map.</summary>
        public abstract EntityList Entities { get; }

        internal Map(MapData mapData)
        {
            _mapData = mapData;
        }

        internal virtual IMapEntity CreateNewEntity(string className) => _mapData.CreateNewEntity(className);

        internal virtual int IndexOf(IMapEntity entity) => _mapData.IndexOf(entity);

        internal virtual void Add(IMapEntity entity) => _mapData.Add(entity);

        internal virtual void Remove(IMapEntity entity) => _mapData.Remove(entity);

        /// <summary>Serializes this map to the given stream.</summary>
        public void Serialize(Stream stream) => _mapData.Serialize(stream);

        internal static Map Create(MapData mapData)
        {
            return new DefaultMap(mapData);
        }

        /// <summary>
        /// Wraps this map instance with a map that logs all changes to the given logger.
        /// </summary>
        /// <param name="logger">Logger to use.</param>
        /// <exception cref="InvalidOperationException">If this map is already wrapped for logging.</exception>
        public Map WithLogger(ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            if (this is LoggingMap)
            {
                throw new InvalidOperationException("Don't wrap maps that are already wrapped");
            }

            return new LoggingMap(_mapData, logger);
        }
    }
}
