using HalfLife.UnifiedSdk.Utilities.Logging;
using Sledge.Formats.Bsp;
using System.CommandLine;

namespace HalfLife.UnifiedSdk.Bsp2Obj
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var bspFileNameArgument = new Argument<FileInfo>("filename", description: "Path to the BSP file");

            var destinationDirectoryOption = new Option<DirectoryInfo?>("--destination",
                getDefaultValue: () => null,
                description: "Directory to save the OBJ, material and texture files to."
                + " If not provided the files will be saved to the directory that the source file is located in");

            var rootCommand = new RootCommand("Half-Life Unified SDK BSP to OBJ converter")
            {
                bspFileNameArgument,
                destinationDirectoryOption
            };

            rootCommand.SetHandler((bspFileName, destinationDirectory, logger) =>
            {
                if (!bspFileName.Exists)
                {
                    logger.Error("The given bsp file \"{BspFileName}\" does not exist", bspFileName);
                    return;
                }

                destinationDirectory ??= bspFileName.Directory;

                if (destinationDirectory is null)
                {
                    logger.Error("Destination directory is invalid");
                    return;
                }

                BspFile bspFile;

                try
                {
                    logger.Information("Reading BSP file {FileName}", bspFileName.FullName);
                    using var stream = bspFileName.OpenRead();
                    bspFile = new(stream);
                }
                catch (Exception e)
                {
                    logger.Error(e, "An error occurred while reading the BSP file");
                    return;
                }

                try
                {
                    destinationDirectory.Create();
                }
                catch (Exception e)
                {
                    logger.Error(e, "An error occurred while creating the destination directory");
                    return;
                }

                logger.Information("Converting BSP data to OBJ");
                
                try
                {
                    BspToObjConverter.Convert(logger, destinationDirectory.FullName, bspFileName.FullName, bspFile);
                }
                catch (Exception e)
                {
                    logger.Error(e, "An error occurred while writing the obj file");
                    return;
                }
            }, bspFileNameArgument, destinationDirectoryOption, LoggerBinder.Instance);

            return await rootCommand.InvokeAsync(args);
        }
    }
}