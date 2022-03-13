using HalfLife.UnifiedSdk.Utilities.Maps;
using Sledge.Formats.Map.Formats;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeMapFile
{
    internal sealed class QuakeMapFileSerializer : IMapSerializer
    {
        private readonly QuakeMapFormat _format = new();

        public string Extension { get; }

        public QuakeMapFileSerializer()
        {
            Extension = "." + _format.Extension;
        }

        public Map Deserialize(string fileName, Stream stream)
        {
            var mapFile = _format.Read(stream);

            return new MapFileMap(fileName, mapFile, _format, "Worldcraft");
        }
    }
}
