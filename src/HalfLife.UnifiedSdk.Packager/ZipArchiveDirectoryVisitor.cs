using System.Collections.Immutable;
using System.IO.Compression;

namespace HalfLife.UnifiedSdk.Packager
{
    internal struct ZipArchiveDirectoryVisitor
    {
        public ZipArchive Archive { get; init; }

        public ImmutableHashSet<string> FilesToExclude { get; init; }

        public string BaseName { get; init; }

        public string EntryName { get; init; }

        private void CreateEntryFromAny(string sourceName)
        {
            if (FilesToExclude.Contains(sourceName))
            {
                Console.WriteLine($"Skipping file or directory \"{sourceName}\"");
                return;
            }

            if (File.GetAttributes(sourceName).HasFlag(FileAttributes.Directory))
            {
                foreach (var file in Directory.GetFiles(sourceName).Concat(Directory.GetDirectories(sourceName)).ToArray())
                {
                    CreateEntryFromAny(file);
                }
            }
            else
            {
                var entryName = EntryName != "" ? sourceName.Replace(BaseName, EntryName) : "";
                Archive.CreateEntryFromFile(sourceName, entryName);
            }
        }

        public static void CreateEntryFromAny(ZipArchive archive, ImmutableHashSet<string> filesToExclude, string sourceName, string entryName = "")
        {
            var visitor = new ZipArchiveDirectoryVisitor
            {
                Archive = archive,
                FilesToExclude = filesToExclude,
                BaseName = sourceName,
                EntryName = entryName
            };

            visitor.CreateEntryFromAny(sourceName);
        }
    }
}
