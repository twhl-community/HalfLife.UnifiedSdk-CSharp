using HalfLife.UnifiedSdk.Utilities.Maps;
using Sledge.Formats.Bsp;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile
{
    internal sealed class BSPSerializer : IMapSerializer
    {
        public string Extension => ".bsp";

        public Map Deserialize(string fileName, Stream stream)
        {
            var bspFile = new BspFile(stream);

            //Force this off so bsp files saved to disk can be loaded when used in normal maps.
            //This means Blue Shift maps can't be saved back to the original game directory,
            //but that's not something you should be doing with this tool.
            bspFile.Options.UseBlueShiftFormat = false;

            return new BSPMap(fileName, bspFile);
        }
    }
}
