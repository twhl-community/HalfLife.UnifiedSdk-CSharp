using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using Semver;
using System.IO;
using System.Text;
using Xunit;

namespace HalfLife.UnifiedSdk.Utilities.Tests
{
    public class MapUpgradeToolTests
    {
        private static Map LoadMap(string fileName, string contents)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
            return MapFormats.Ent.Deserialize(fileName, stream);
        }

        private static Map LoadSimpleMapWithNoVersion() => LoadMap("Simple map with no version", KeyValueUtilities.EmptyMapString);

        [Fact]
        public void NoActions_OnlyAddsVersionKey()
        {
            var map = LoadSimpleMapWithNoVersion();

            var upgradeTool = new MapUpgradeTool();

            upgradeTool.Upgrade(new MapUpgrade(map));

            Assert.Single(map.Entities);

            Assert.Equal(2, map.Entities.Worldspawn.Count);

            Assert.True(map.Entities.Worldspawn.ContainsKey(upgradeTool.GameVersionKey));
            Assert.Equal(MapUpgradeTool.FirstVersion.ToString(), map.Entities.Worldspawn.GetString(upgradeTool.GameVersionKey));
        }

        [Fact]
        public void OneAction_AddMaxRange_PerformsUpgrade()
        {
            var map = LoadSimpleMapWithNoVersion();

            var action = new MapUpgradeAction(new SemVersion(1, 0, 0));

            action.Upgrading += (context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 32768);

            var upgradeTool = new MapUpgradeTool(action);

            upgradeTool.Upgrade(new MapUpgrade(map));

            Assert.Single(map.Entities);

            Assert.Equal(3, map.Entities.Worldspawn.Count);

            Assert.True(map.Entities.Worldspawn.ContainsKey(upgradeTool.GameVersionKey));
            Assert.Equal(action.Version.ToString(), map.Entities.Worldspawn.GetString(upgradeTool.GameVersionKey));

            Assert.True(map.Entities.Worldspawn.ContainsKey("MaxRange"));
            Assert.Equal(32768, map.Entities.Worldspawn.GetInteger("MaxRange"));
        }

        [Fact]
        public void OneAction_AddMaxRangeToNewerMap_DoesNothing()
        {
            var map = LoadSimpleMapWithNoVersion();

            //Set version to 2.0.0.
            map.Entities.Worldspawn.SetString(MapUpgradeTool.DefaultGameVersionKey, "2.0.0");

            var action = new MapUpgradeAction(new SemVersion(1, 0, 0));

            action.Upgrading += (context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 32768);

            var upgradeTool = new MapUpgradeTool(action);

            var (from, to) = upgradeTool.Upgrade(new MapUpgrade(map));

            Assert.Equal(from, to);

            Assert.Single(map.Entities);

            Assert.Equal(2, map.Entities.Worldspawn.Count);

            Assert.True(map.Entities.Worldspawn.ContainsKey(upgradeTool.GameVersionKey));
            Assert.Equal("2.0.0", map.Entities.Worldspawn.GetString(upgradeTool.GameVersionKey));
        }

        [Fact]
        public void OneAction_UpgradeNewerMapFromOlderVersion_Throws()
        {
            var map = LoadSimpleMapWithNoVersion();

            //Set version to 2.0.0.
            map.Entities.Worldspawn.SetString(MapUpgradeTool.DefaultGameVersionKey, "2.0.0");

            var upgradeTool = new MapUpgradeTool();

            Assert.Throws<MapUpgradeException>(() => upgradeTool.Upgrade(new MapUpgrade(map)
            {
                From = new SemVersion(1, 0, 0)
            }));

            Assert.Single(map.Entities);

            Assert.Equal(2, map.Entities.Worldspawn.Count);

            Assert.True(map.Entities.Worldspawn.ContainsKey(upgradeTool.GameVersionKey));
            Assert.Equal("2.0.0", map.Entities.Worldspawn.GetString(upgradeTool.GameVersionKey));
        }

        [Fact]
        public void TwoActions_AddAndUpdateMaxRange_PerformsUpgradeForBoth()
        {
            var map = LoadSimpleMapWithNoVersion();

            //Version 1.0.0: set MaxRange to 4096 (default value in original fgd).
            var action = new MapUpgradeAction(new SemVersion(1, 0, 0));

            action.Upgrading += (context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 4096);

            //Version 2.0.0: set MaxRange to 32768.
            var action2 = new MapUpgradeAction(new SemVersion(2, 0, 0));

            action2.Upgrading += (context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 32768);

            var upgradeTool = new MapUpgradeTool(action, action2);

            upgradeTool.Upgrade(new MapUpgrade(map));

            Assert.Single(map.Entities);

            Assert.Equal(3, map.Entities.Worldspawn.Count);

            Assert.True(map.Entities.Worldspawn.ContainsKey(upgradeTool.GameVersionKey));
            Assert.Equal(action2.Version.ToString(), map.Entities.Worldspawn.GetString(upgradeTool.GameVersionKey));

            Assert.True(map.Entities.Worldspawn.ContainsKey("MaxRange"));
            Assert.Equal(32768, map.Entities.Worldspawn.GetInteger("MaxRange"));
        }

        [Fact]
        public void TwoUpgrades_DoesNotApplySecondUpgrade()
        {
            var map = LoadSimpleMapWithNoVersion();

            //First upgrade from 0.0.0 to 1.0.0.
            {
                var action = new MapUpgradeAction(new SemVersion(1, 0, 0));

                action.Upgrading += (context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 32768);

                var upgradeTool = new MapUpgradeTool(action);

                upgradeTool.Upgrade(new MapUpgrade(map));

                RunTests(upgradeTool, action);
            }

            //Second upgrade from 1.0.0 to 1.0.0, should do nothing.
            {
                var action = new MapUpgradeAction(new SemVersion(1, 0, 0));

                //Set MaxRange to a different value.
                action.Upgrading += (context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 4096);

                var upgradeTool = new MapUpgradeTool(action);

                upgradeTool.Upgrade(new MapUpgrade(map));

                RunTests(upgradeTool, action);
            }

            void RunTests(MapUpgradeTool upgradeTool, MapUpgradeAction action)
            {
                Assert.Single(map.Entities);

                Assert.Equal(3, map.Entities.Worldspawn.Count);

                Assert.True(map.Entities.Worldspawn.ContainsKey(upgradeTool.GameVersionKey));
                Assert.Equal(action.Version.ToString(), map.Entities.Worldspawn.GetString(upgradeTool.GameVersionKey));

                Assert.True(map.Entities.Worldspawn.ContainsKey("MaxRange"));
                Assert.Equal(32768, map.Entities.Worldspawn.GetInteger("MaxRange"));
            }
        }
    }
}
