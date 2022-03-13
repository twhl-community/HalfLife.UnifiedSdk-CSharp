using HalfLife.UnifiedSdk.Utilities.Maps;
using Sledge.Formats.Map.Formats;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile
{
    internal sealed class WorldcraftRmfMapFileSerializer : IMapSerializer
    {
        private readonly WorldcraftRmfFormat _format = new();

        public string Extension { get; }

        public WorldcraftRmfMapFileSerializer()
        {
            Extension = "." + _format.Extension;
        }

        public Map Deserialize(string fileName, Stream stream)
        {
            var mapFile = _format.Read(stream);

            return new MapFileMap(fileName, mapFile, _format, "2.2");
        }
    }
}
