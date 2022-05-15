using System.Collections.Immutable;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Packager
{
    /// <summary>
    /// Creates a mod package from a mod installation.
    /// </summary>
    sealed class Packager : IDisposable
    {
        public const string PackageExtension = ".zip";

        private readonly ImmutableHashSet<string> _filesToExclude;
        private readonly ZipArchive _archive;

        public string PackageName { get; }

        public Packager(string packageName, IEnumerable<string> filesToExclude)
        {
            PackageName = packageName + PackageExtension;

            _filesToExclude = filesToExclude.ToImmutableHashSet();

            Console.WriteLine($"Creating archive {PackageName}");
            _archive = ZipFile.Open(PackageName, ZipArchiveMode.Create);
        }

        public void Dispose() => _archive.Dispose();

        public void AddFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var completePath = file;

                //If the file doesn't exist then it might be game-specific, so the packager should just ignore it.
                if (!File.Exists(completePath) && !Directory.Exists(completePath))
                {
                    Console.WriteLine($"Skipping \"{completePath}\" because it does not exist");
                    continue;
                }

                if (File.GetAttributes(completePath).HasFlag(FileAttributes.Directory))
                {
                    Console.WriteLine($"Adding directory \"{completePath}\"");
                }
                else
                {
                    Console.WriteLine($"Adding file \"{completePath}\"");
                }

                var newName = completePath;

                // Files ending with ".install" need to be renamed.
                newName = Regex.Replace(newName, "\\.install$", "");

                if (completePath != newName)
                {
                    Console.WriteLine($"Renaming \"{completePath}\" to \"{newName}\"");
                }

                ZipArchiveDirectoryVisitor.CreateEntryFromAny(_archive, _filesToExclude, completePath, newName);
            }
        }
    }
}
