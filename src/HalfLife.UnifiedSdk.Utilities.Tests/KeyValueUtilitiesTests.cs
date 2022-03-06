using HalfLife.UnifiedSdk.Utilities.Tools;
using Xunit;

namespace HalfLife.UnifiedSdk.Utilities.Tests
{
    public class KeyValueUtilitiesTests
    {
        [Fact]
        public void EmptyMapString_ResultMatches()
        {
            Assert.Equal(@"{
""classname"" ""worldspawn""
}
",
                KeyValueUtilities.EmptyMapString);
        }
    }
}
