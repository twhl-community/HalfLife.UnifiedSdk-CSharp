using HalfLife.UnifiedSdk.Utilities.Entities;
using System.IO;
using System.Text;

namespace HalfLife.UnifiedSdk.Utilities.Tools
{
    /// <summary>Provides Ripent functionality.</summary>
    public static class Ripent
    {
        /// <summary>Creates a <c>.ent</c> file from a BSP file with the given name.</summary>
        public static void Export(string bspFileName, string entFileName)
        {
            var bspMap = MapFormats.Deserialize(bspFileName);

            //Create an empty .ent map.
            var entMap = MapFormats.Ent.Deserialize(entFileName, new MemoryStream(Encoding.UTF8.GetBytes(KeyValueUtilities.EmptyMapString)));

            entMap.Entities.ReplaceWith(bspMap.Entities);

            using var stream = File.Open(entFileName, FileMode.Create, FileAccess.Write);

            entMap.Serialize(stream);
        }

        /// <summary>Creates a <c>.ent</c> file from a BSP file with the same name.</summary>
        public static void Export(string bspFileName)
        {
            var entFileName = Path.ChangeExtension(bspFileName, MapFormats.Ent.Extension);
            Export(bspFileName, entFileName);
        }

        /// <summary>Overwrites the given BSP file's entity data with a <c>.ent</c> file with the given name.</summary>
        public static void Import(string bspFileName, string entFileName)
        {
            //Always deserialize as bsp.
            var bspMap = MapFormats.Deserialize(bspFileName, MapFormats.Bsp);
            var entMap = MapFormats.Deserialize(entFileName, MapFormats.Ent);

            bspMap.Entities.ReplaceWith(entMap.Entities);

            using var stream = File.Open(bspFileName, FileMode.Create, FileAccess.Write);

            bspMap.Serialize(stream);
        }

        /// <summary>Overwrites the given BSP file's entity data with a <c>.ent</c> file with the same name.</summary>
        public static void Import(string bspFileName)
        {
            var entFileName = Path.ChangeExtension(bspFileName, MapFormats.Ent.Extension);
            Import(bspFileName, entFileName);
        }
    }
}
