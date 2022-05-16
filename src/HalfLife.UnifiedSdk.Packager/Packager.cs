using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace HalfLife.UnifiedSdk.Packager
{
    /// <summary>
    /// Creates a mod package from a mod installation.
    /// </summary>
    internal sealed class Packager : IDisposable
    {
        public const string PackageExtension = ".zip";

        private readonly IConsole _console;
        private readonly ImmutableHashSet<string> _filesToExclude;
        private readonly ZipArchive _archive;

        public string PackageName { get; }

        public Packager(IConsole console, string packageName, IEnumerable<string> filesToExclude)
        {
            _console = console;
            PackageName = packageName + PackageExtension;

            _filesToExclude = filesToExclude.ToImmutableHashSet();

            _console.Out.WriteLine($"Creating archive {PackageName}");
            _archive = ZipFile.Open(PackageName, ZipArchiveMode.Create);
        }

        public void Dispose() => _archive.Dispose();

        public void AddFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var completePath = file;

                if (!File.Exists(completePath) && !Directory.Exists(completePath))
                {
                    _archive.Dispose();
                    File.Delete(PackageName);
                    throw new PackagerException($"\"{completePath}\" does not exist");
                }

                if (File.GetAttributes(completePath).HasFlag(FileAttributes.Directory))
                {
                    _console.Out.WriteLine($"Adding directory \"{completePath}\"");
                }
                else
                {
                    _console.Out.WriteLine($"Adding file \"{completePath}\"");
                }

                var newName = completePath;

                // Files ending with ".install" need to be renamed.
                newName = Regex.Replace(newName, "\\.install$", "");

                if (completePath != newName)
                {
                    _console.Out.WriteLine($"Renaming \"{completePath}\" to \"{newName}\"");
                }

                ZipArchiveDirectoryVisitor.CreateEntryFromAny(_console, _archive, _filesToExclude, completePath, newName);
            }
        }
    }
}
