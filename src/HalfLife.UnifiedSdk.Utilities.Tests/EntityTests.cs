using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Serialization.EntFile;
using HalfLife.UnifiedSdk.Utilities.Tools;
using System;
using Xunit;

namespace HalfLife.UnifiedSdk.Utilities.Tests
{
    public class EntityTests
    {
        private static Map CreateEmptyMap()
        {
            var entities = new Sledge.Formats.Bsp.Lumps.Entities
            {
                new Sledge.Formats.Bsp.Objects.Entity
                {
                    ClassName = KeyValueUtilities.WorldspawnClassName
                }
            };

            return new EntMap("EmptyMap", entities);
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
            map.Entities.Add(entity);

            Assert.Throws<ArgumentException>(() => entity.ClassName = KeyValueUtilities.WorldspawnClassName);
        }
    }
}
