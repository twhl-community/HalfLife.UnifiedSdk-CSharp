using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Compression;

namespace HalfLife.UnifiedSdk.Packager
{
    internal struct ZipArchiveDirectoryVisitor
    {
        private readonly IConsole _console;

        private readonly ZipArchive _archive;

        private readonly ImmutableHashSet<string> _filesToExclude;

        private readonly string _baseName;

        private readonly string _entryName;

        private ZipArchiveDirectoryVisitor(IConsole console, ZipArchive archive, ImmutableHashSet<string> filesToExclude,
            string baseName, string entryName)
        {
            _console = console;
            _archive = archive;
            _filesToExclude = filesToExclude;
            _baseName = baseName;
            _entryName = entryName;
        }

        private void CreateEntryFromAny(string sourceName)
        {
            if (_filesToExclude.Contains(sourceName))
            {
                _console.Out.WriteLine($"Skipping file or directory \"{sourceName}\"");
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
                var entryName = _entryName != "" ? sourceName.Replace(_baseName, _entryName) : "";
                _archive.CreateEntryFromFile(sourceName, entryName);
            }
        }

        public static void CreateEntryFromAny(
            IConsole console, ZipArchive archive, ImmutableHashSet<string> filesToExclude, string sourceName, string entryName = "")
        {
            var visitor = new ZipArchiveDirectoryVisitor(console, archive, filesToExclude, sourceName, entryName);

            visitor.CreateEntryFromAny(sourceName);
        }
    }
}
