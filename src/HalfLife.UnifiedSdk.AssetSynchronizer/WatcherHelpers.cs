using Serilog;

namespace HalfLife.UnifiedSdk.AssetSynchronizer
{
    internal static class WatcherHelpers
    {
        public static void PrintException(ILogger logger, Exception? ex)
        {
            if (ex is not null)
            {
                logger.Error("Message: {Message}", ex.Message);
                logger.Error("Stacktrace:");

                if (ex.StackTrace is not null)
                {
                    logger.Error("{StackTrace}", ex.StackTrace);
                }
                else
                {
                    logger.Error("No stack trace");
                }

                PrintException(logger, ex.InnerException);
            }
        }
    }
}
