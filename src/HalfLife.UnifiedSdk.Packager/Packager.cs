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
    internal static class Packager
    {
        public const string PackageExtension = ".zip";

        public static void CreatePackage(IConsole console, string packageName,
            IEnumerable<string> filesToInclude, ImmutableHashSet<string> filesToExclude)
        {
            var completePackageName = packageName + PackageExtension;

            try
            {
                CreatePackageCore(console, completePackageName, filesToInclude, filesToExclude);
            }
            catch(PackagerException)
            {
                File.Delete(completePackageName);
                throw;
            }
        }

        private static void CreatePackageCore(IConsole console, string completePackageName,
            IEnumerable<string> filesToInclude, ImmutableHashSet<string> filesToExclude)
        {
            console.Out.WriteLine($"Creating archive {completePackageName}");
            using var archive = ZipFile.Open(completePackageName, ZipArchiveMode.Create);

            foreach (var file in filesToInclude)
            {
                if (!File.Exists(file) && !Directory.Exists(file))
                {
                    throw new PackagerException($"\"{file}\" does not exist");
                }

                if (File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                {
                    console.Out.WriteLine($"Adding directory \"{file}\"");
                }
                else
                {
                    console.Out.WriteLine($"Adding file \"{file}\"");
                }

                var newName = file;

                // Files ending with ".install" need to be renamed.
                newName = Regex.Replace(newName, "\\.install$", "");

                if (file != newName)
                {
                    console.Out.WriteLine($"Renaming \"{file}\" to \"{newName}\"");
                }

                ZipArchiveDirectoryVisitor.CreateEntryFromAny(console, archive, filesToExclude, file, newName);
            }
        }
    }
}
