using System.CommandLine;
using System.CommandLine.IO;

namespace HalfLife.UnifiedSdk.AssetSynchronizer
{
    internal static class WatcherHelpers
    {
        public static void PrintException(IConsole console, Exception? ex)
        {
            if (ex is not null)
            {
                console.Error.WriteLine($"Message: {ex.Message}");
                console.Error.WriteLine("Stacktrace:");
                console.Error.WriteLine(ex.StackTrace ?? "No stack trace");
                console.Error.WriteLine();
                PrintException(console, ex.InnerException);
            }
        }
    }
}
