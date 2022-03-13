using HalfLife.UnifiedSdk.Utilities.Maps;
using Sledge.Formats.Bsp;
using System.IO;
using System.Text;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.EntFile
{
    internal sealed class EntSerializer : IMapSerializer
    {
        public string Extension => ".ent";

        public Map Deserialize(string fileName, Stream stream)
        {
            var entitiesLump = new Sledge.Formats.Bsp.Lumps.Entities();

            using var reader = new BinaryReader(stream, Encoding.UTF8, true);
            entitiesLump.Read(reader, new Blob
            {
                Offset = 0,
                Length = (int)stream.Length
            },
            Version.Goldsource);

            return new EntMap(fileName, entitiesLump);
        }
    }
}
