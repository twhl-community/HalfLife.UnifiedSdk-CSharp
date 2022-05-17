using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Maps;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using Semver;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace HalfLife.UnifiedSdk.Utilities.Tests
{
    public class MapUpgradeToolTests
    {
        private static readonly SemVersion _testVersion = new(1, 0, 0);

        private static Map LoadMap(string fileName, string contents)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
            return MapFormats.Ent.Deserialize(fileName, stream);
        }

        private static Map LoadSimpleMapWithNoVersion() => LoadMap("Simple map with no version", KeyValueUtilities.EmptyMapString);

        private static MapUpgradeTool ToolWithDelegate(Action<MapUpgradeContext> action)
        {
            return MapUpgradeToolBuilder.Build(builder => builder.AddUpgrade(_testVersion, upgrade => upgrade.AddAction(action)));
        }

        [Fact]
        public void NoActions_OnlyAddsVersionKey()
        {
            var map = LoadSimpleMapWithNoVersion();

            var upgradeTool = MapUpgradeToolBuilder.Build(_ => { });

            upgradeTool.Upgrade(new MapUpgradeCommand(map));

            Assert.Single(map.Entities);

            Assert.Equal(2, map.Entities.Worldspawn.Count);

            Assert.True(map.Entities.Worldspawn.ContainsKey(upgradeTool.GameVersionKey));
            Assert.Equal(MapUpgradeTool.FirstVersion.ToString(), map.Entities.Worldspawn.GetString(upgradeTool.GameVersionKey));
        }

        [Fact]
        public void OneAction_AddMaxRange_PerformsUpgrade()
        {
            var map = LoadSimpleMapWithNoVersion();

            var upgradeTool = ToolWithDelegate((context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 32768));

            upgradeTool.Upgrade(new MapUpgradeCommand(map));

            Assert.Single(map.Entities);

            Assert.Equal(3, map.Entities.Worldspawn.Count);

            Assert.True(map.Entities.Worldspawn.ContainsKey(upgradeTool.GameVersionKey));
            Assert.Equal(_testVersion.ToString(), map.Entities.Worldspawn.GetString(upgradeTool.GameVersionKey));

            Assert.True(map.Entities.Worldspawn.ContainsKey("MaxRange"));
            Assert.Equal(32768, map.Entities.Worldspawn.GetInteger("MaxRange"));
        }

        [Fact]
        public void OneAction_AddMaxRangeToNewerMap_DoesNothing()
        {
            var map = LoadSimpleMapWithNoVersion();

            //Set version to 2.0.0.
            map.Entities.Worldspawn.SetString(MapUpgradeTool.DefaultGameVersionKey, "2.0.0");

            var upgradeTool = ToolWithDelegate((context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 32768));

            var (from, to) = upgradeTool.Upgrade(new MapUpgradeCommand(map));

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

            var upgradeTool = MapUpgradeToolBuilder.Build(_ => { });

            Assert.Throws<MapUpgradeException>(() => upgradeTool.Upgrade(new MapUpgradeCommand(map)
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

            var version2 = new SemVersion(2, 0, 0);

            var upgradeTool = MapUpgradeToolBuilder.Build(builder =>
            {
                //Version 1.0.0: set MaxRange to 4096 (default value in original fgd).
                builder.AddUpgrade(_testVersion, upgrade => upgrade.AddAction((context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 4096)));

                //Version 2.0.0: set MaxRange to 32768.
                builder.AddUpgrade(version2, upgrade => upgrade.AddAction((context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 32768)));
            });

            upgradeTool.Upgrade(new MapUpgradeCommand(map));

            Assert.Single(map.Entities);

            Assert.Equal(3, map.Entities.Worldspawn.Count);

            Assert.True(map.Entities.Worldspawn.ContainsKey(upgradeTool.GameVersionKey));
            Assert.Equal(version2.ToString(), map.Entities.Worldspawn.GetString(upgradeTool.GameVersionKey));

            Assert.True(map.Entities.Worldspawn.ContainsKey("MaxRange"));
            Assert.Equal(32768, map.Entities.Worldspawn.GetInteger("MaxRange"));
        }

        [Fact]
        public void TwoUpgrades_DoesNotApplySecondUpgrade()
        {
            var map = LoadSimpleMapWithNoVersion();

            //First upgrade from 0.0.0 to 1.0.0.
            {
                var upgradeTool = ToolWithDelegate((context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 32768));

                upgradeTool.Upgrade(new MapUpgradeCommand(map));

                RunTests(upgradeTool, upgradeTool.Upgrades[0]);
            }

            //Second upgrade from 1.0.0 to 1.0.0, should do nothing.
            {
                //Set MaxRange to a different value.
                var upgradeTool = ToolWithDelegate((context) => context.Map.Entities.Worldspawn.SetInteger("MaxRange", 4096));

                upgradeTool.Upgrade(new MapUpgradeCommand(map));

                RunTests(upgradeTool, upgradeTool.Upgrades[0]);
            }

            void RunTests(MapUpgradeTool upgradeTool, MapUpgrade action)
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
