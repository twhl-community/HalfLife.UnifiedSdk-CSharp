using System;
using System.Collections.Generic;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    internal abstract class MapData
    {
        public string FileName { get; }

        public MapContentType ContentType { get; }

        protected MapData(string fileName, MapContentType contentType)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            ContentType = contentType;
        }

        public abstract IEnumerable<IMapEntity> GetEntities();

        public abstract IMapEntity CreateNewEntity(string className);

        public abstract int IndexOf(IMapEntity entity);

        public abstract void Add(IMapEntity entity);

        public abstract void Remove(IMapEntity entity);

        public abstract void Serialize(Stream stream);
    }
}
