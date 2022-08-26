using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Tools;
using System;
using Xunit;

namespace HalfLife.UnifiedSdk.Utilities.Tests
{
    public class EntityTests
    {
        private static Map CreateEmptyMap()
        {
            return MapFormats.CreateEntMap("EmptyMap");
        }

        [Fact]
        public void Worldspawn_ChangingClassNameThroughProperty_Throws()
        {
            var map = CreateEmptyMap();

            var worldspawn = map.Entities.Worldspawn;

            Assert.Throws<ArgumentException>(() => worldspawn.ClassName = "foo");
        }

        [Fact]
        public void Worldspawn_ChangingClassNameThroughIndexer_Throws()
        {
            var map = CreateEmptyMap();

            var worldspawn = map.Entities.Worldspawn;

            Assert.Throws<ArgumentException>(() => worldspawn[KeyValueUtilities.ClassName] = "foo");
        }

        [Fact]
        public void Worldspawn_ChangingClassNameThroughSetString_Throws()
        {
            var map = CreateEmptyMap();

            var worldspawn = map.Entities.Worldspawn;

            Assert.Throws<ArgumentException>(() => worldspawn.SetString(KeyValueUtilities.ClassName, "foo"));
        }

        [Fact]
        public void NotWorldspawn_ChangingClassNameToWorldspawn_Throws()
        {
            var map = CreateEmptyMap();

            var entity = map.Entities.CreateNewEntity("foo");

            Assert.Throws<ArgumentException>(() => entity.ClassName = KeyValueUtilities.WorldspawnClassName);
        }
    }
}
