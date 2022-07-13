using HalfLife.UnifiedSdk.Utilities.Tools;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace HalfLife.UnifiedSdk.Utilities.Tests
{
    public class MapFormatsTests
    {
        private readonly ITestOutputHelper _output;

        public MapFormatsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void EmptyMapString_CreatesEmptyMap()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(KeyValueUtilities.EmptyMapString));

            var map = MapFormats.Deserialize("test.ent", stream);

            Assert.Single(map.Entities);

            Assert.Equal("worldspawn", map.Entities.Worldspawn.ClassName);
            Assert.Equal("worldspawn", map.Entities[0].ClassName);
        }

        [Fact]
        public void EmptyMapString_WritesEmptyMap()
        {
            var emptyMapBytes = Encoding.UTF8.GetBytes(KeyValueUtilities.EmptyMapString);

            var stream = new MemoryStream(emptyMapBytes);

            var map = MapFormats.Deserialize("test.ent", stream);

            using var destination = new MemoryStream();

            map.Serialize(destination);

            var resultText = Encoding.UTF8.GetString(destination.ToArray());

            _output.WriteLine(resultText);

            //EmptyMapString uses line endings based on the platform the code was compiled on
            //while the writer always uses \n, so ignore the differences.
            Assert.Equal(KeyValueUtilities.EmptyMapString + '\0', resultText, ignoreLineEndingDifferences: true);
        }
    }
}
