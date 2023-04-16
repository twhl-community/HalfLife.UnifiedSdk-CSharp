using Newtonsoft.Json;

namespace HalfLife.UnifiedSdk.MapCfgGenerator
{
    internal sealed record Section(Action<JsonTextWriter> WriterCallback, string Condition = "");
}
