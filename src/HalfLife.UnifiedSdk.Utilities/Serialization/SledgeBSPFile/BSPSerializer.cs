using HalfLife.UnifiedSdk.Utilities.Maps;
using Sledge.Formats.Bsp;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile
{
    internal sealed class BSPSerializer : IMapSerializer
    {
        public string Extension => "bsp";

        public Map Deserialize(string fileName, Stream stream)
        {
            var bspFile = new BspFile(stream);
            return new BSPMap(fileName, bspFile);
        }
    }
}
