using HalfLife.UnifiedSdk.Utilities.Serialization.SledgeBSPFile;
using Sledge.Formats.Bsp;
using System.IO;
using System.Text;

namespace HalfLife.UnifiedSdk.Utilities.Serialization.EntFile
{
    /// <summary>A Ripent <c>.ent</c> file. Contains only entity data in compiled form.</summary>
    internal sealed class EntMap : BSPMapBase
    {
        /// <inheritdoc/>
        public EntMap(string fileName, Sledge.Formats.Bsp.Lumps.Entities entitiesLump)
            : base(fileName, entitiesLump)
        {
        }

        /// <inheritdoc/>
        public override void Serialize(Stream stream)
        {
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
            _entitiesLump.Write(writer, Version.Goldsource);
        }
    }
}
