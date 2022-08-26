using Sledge.Formats.Bsp;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile
{
    internal sealed class BSPMap : BSPMapBase
    {
        private readonly BspFile _bspFile;

        internal BSPMap(string fileName, BspFile bspFile)
            : base(fileName,
                  bspFile.GetLump<Sledge.Formats.Bsp.Lumps.Entities>() ?? throw new InvalidFormatException("Missing entities lump"))
        {
            _bspFile = bspFile;
        }

        public override void Serialize(Stream stream)
        {
            _bspFile.WriteToStream(stream, _bspFile.Version);
        }
    }
}
