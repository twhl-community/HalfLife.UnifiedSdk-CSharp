using System.Collections.Immutable;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Games
{
    /// <summary>Contains data about Valve games.</summary>
    public static class ValveGames
    {
        //These map name lists are hardcoded, but they could be loaded from a file instead.
        //That's only useful for games that are still getting new maps.

        /// <summary>Half-Life 1.</summary>
        public static GameInfo HalfLife1 { get; } = new GameInfo(
            GameEngine.GoldSource,
            "Half-Life",
            "valve",
            () => new[]
                {
                    "c0a0",
                    "c0a0a",
                    "c0a0b",
                    "c0a0c",
                    "c0a0d",
                    "c0a0e",
                    "c1a0",
                    "c1a0a",
                    "c1a0b",
                    "c1a0c",
                    "c1a0d",
                    "c1a0e",
                    "c1a1",
                    "c1a1a",
                    "c1a1b",
                    "c1a1c",
                    "c1a1d",
                    "c1a1f",
                    "c1a2",
                    "c1a2a",
                    "c1a2b",
                    "c1a2c",
                    "c1a2d",
                    "c1a3",
                    "c1a3a",
                    "c1a3b",
                    "c1a3c",
                    "c1a3d",
                    "c1a4",
                    "c1a4b",
                    "c1a4d",
                    "c1a4e",
                    "c1a4f",
                    "c1a4g",
                    "c1a4i",
                    "c1a4j",
                    "c1a4k",
                    "c2a1",
                    "c2a1a",
                    "c2a1b",
                    "c2a2",
                    "c2a2a",
                    "c2a2b1",
                    "c2a2b2",
                    "c2a2c",
                    "c2a2d",
                    "c2a2e",
                    "c2a2f",
                    "c2a2g",
                    "c2a2h",
                    "c2a3",
                    "c2a3a",
                    "c2a3b",
                    "c2a3c",
                    "c2a3d",
                    "c2a3e",
                    "c2a4",
                    "c2a4a",
                    "c2a4b",
                    "c2a4c",
                    "c2a4d",
                    "c2a4e",
                    "c2a4f",
                    "c2a4g",
                    "c2a5",
                    "c2a5a",
                    "c2a5b",
                    "c2a5c",
                    "c2a5d",
                    "c2a5e",
                    "c2a5f",
                    "c2a5g",
                    "c2a5w",
                    "c2a5x",
                    "c3a1",
                    "c3a1a",
                    "c3a1b",
                    "c3a2",
                    "c3a2a",
                    "c3a2b",
                    "c3a2c",
                    "c3a2d",
                    "c3a2e",
                    "c3a2f",
                    "c4a1",
                    "c4a1a",
                    "c4a1b",
                    "c4a1c",
                    "c4a1d",
                    "c4a1e",
                    "c4a1f",
                    "c4a2",
                    "c4a2a",
                    "c4a2b",
                    "c4a3",
                    "c5a1"
                }
            .Select(n => new MapInfo(n, MapCategory.Campaign))
            .Concat(new[]
                {
                    "t0a0",
                    "t0a0a",
                    "t0a0b",
                    "t0a0b1",
                    "t0a0b2",
                    "t0a0c",
                    "t0a0d"
                }
            .Select(n => new MapInfo(n, MapCategory.Training)))
            .Concat(new[]
                {
                    "boot_camp",
                    "bounce",
                    "crossfire",
                    "datacore",
                    "frenzy",
                    "gasworks",
                    "lambda_bunker",
                    "rapidcore",
                    "snark_pit",
                    "stalkyard",
                    "subtransit",
                    "undertow"
                }
            .Select(n => new MapInfo(n, MapCategory.Multiplayer)))
            .ToImmutableDictionary(m => m.Name),
            () => new[]
            {
                "Half-Life01",
                "Half-Life02",
                "Half-Life03",
                "Half-Life04",
                "Half-Life05",
                "Half-Life06",
                "Half-Life07",
                "Half-Life08",
                "Half-Life09",
                "Half-Life10",
                "Half-Life11",
                "Half-Life12",
                "Half-Life13",
                "Half-Life14",
                "Half-Life15",
                "Half-Life16",
                "Half-Life17",
                "Prospero01",
                "Prospero02",
                "Prospero03",
                "Prospero04",
                "Prospero05",
                "Suspense01",
                "Suspense02",
                "Suspense03",
                "Suspense05",
                "Suspense07"
            }.ToImmutableHashSet());

        /// <summary>Half-Life: Opposing Force.</summary>
        public static GameInfo OpposingForce { get; } = new(
            GameEngine.GoldSource,
            "Opposing Force",
            "gearbox",
            () => new[]
                {
                    "of0a0",
                    "of1a1",
                    "of1a2",
                    "of1a3",
                    "of1a4",
                    "of1a4b",
                    "of1a5",
                    "of1a5b",
                    "of1a6",
                    "of2a1",
                    "of2a1b",
                    "of2a2",
                    "of2a3",
                    "of2a4",
                    "of2a5",
                    "of2a6",
                    "of3a1",
                    "of3a1b",
                    "of3a2",
                    "of3a4",
                    "of3a5",
                    "of3a6",
                    "of4a1",
                    "of4a2",
                    "of4a3",
                    "of4a4",
                    "of4a5",
                    "of5a1",
                    "of5a2",
                    "of5a3",
                    "of5a4",
                    "of6a1",
                    "of6a2",
                    "of6a3",
                    "of6a4",
                    "of6a4b",
                    "of6a5",
                    "of7a0"
                }
            .Select(n => new MapInfo(n, MapCategory.Campaign))
            .Concat(new[]
                {
                    "ofboot0",
                    "ofboot1",
                    "ofboot2",
                    "ofboot3",
                    "ofboot4"
                }
            .Select(n => new MapInfo(n, MapCategory.Training)))
            .Concat(new[]
                {
                    "op4cp_park",
                    "op4ctf_biodomes",
                    "op4ctf_chasm",
                    "op4ctf_crash",
                    "op4ctf_dam",
                    "op4ctf_gunyard",
                    "op4ctf_hairball",
                    "op4ctf_mortar",
                    "op4ctf_power",
                    "op4ctf_repent",
                    "op4ctf_wonderland",
                    "op4ctf_xendance",
                    "op4_bootcamp",
                    "op4_datacore",
                    "op4_demise",
                    "op4_disposal",
                    "op4_gasworks",
                    "op4_kbase",
                    "op4_kndyone",
                    "op4_meanie",
                    "op4_outpost",
                    "op4_park",
                    "op4_repent",
                    "op4_rubble",
                    "op4_xendance"
                }
            .Select(n => new MapInfo(n, MapCategory.Multiplayer)))
            .ToImmutableDictionary(m => m.Name),
            () => new[]
            {
                "Half-Life01",
                "Half-Life02",
                "Half-Life03",
                "Half-Life04",
                "Half-Life05",
                "Half-Life07",
                "Half-Life08",
                "Half-Life09",
                "Half-Life10",
                "Half-Life12",
                "Half-Life13",
                "Half-Life14",
                "Half-Life15",
                "Half-Life16",
                "Prospero01",
                "Prospero02",
                "Suspense01",
                "Suspense02",
                "Suspense03"
            }.ToImmutableHashSet());

        /// <summary>Half-Life: Blue Shift.</summary>
        public static GameInfo BlueShift { get; } = new(
            GameEngine.GoldSource,
            "Blue Shift",
            "bshift",
            () => new[]
                {
                    "ba_canal1",
                    "ba_canal1b",
                    "ba_canal2",
                    "ba_canal3",
                    "ba_elevator",
                    "ba_maint",
                    "ba_outro",
                    "ba_power1",
                    "ba_power2",
                    "ba_security1",
                    "ba_security2",
                    "ba_teleport1",
                    "ba_teleport2",
                    "ba_tram1",
                    "ba_tram2",
                    "ba_tram3",
                    "ba_xen1",
                    "ba_xen2",
                    "ba_xen3",
                    "ba_xen4",
                    "ba_xen5",
                    "ba_xen6",
                    "ba_yard1",
                    "ba_yard2",
                    "ba_yard3",
                    "ba_yard3a",
                    "ba_yard3b",
                    "ba_yard4",
                    "ba_yard4a",
                    "ba_yard5",
                    "ba_yard5a"
                }
            .Select(n => new MapInfo(n, MapCategory.Campaign))
            .Concat(new[]
                {
                    "ba_hazard1",
                    "ba_hazard2",
                    "ba_hazard3",
                    "ba_hazard4",
                    "ba_hazard5",
                    "ba_hazard6"
                }
            .Select(n => new MapInfo(n, MapCategory.Training)))
            .ToImmutableDictionary(m => m.Name),
            () => new[]
            {
                "Half-Life01",
                "Half-Life02",
                "Half-Life03",
                "Half-Life04",
                "Half-Life05",
                "Half-Life07",
                "Half-Life08",
                "Half-Life09",
                "Half-Life10",
                "Half-Life12",
                "Half-Life13",
                "Half-Life14",
                "Half-Life15",
                "Half-Life16",
                "Prospero01",
                "Prospero02",
                "Suspense01",
                "Suspense02",
                "Suspense03"
            }.ToImmutableHashSet());

        /// <summary>Half-Life: Uplink.</summary>
        /// <remarks>
        /// Not included in the list of games because you can't install it through Steam,
        /// it's located in a different directory and it uses the same mod directory.
        /// </remarks>
        public static GameInfo HalfLifeUplink { get; } = new GameInfo(
            GameEngine.GoldSource,
            "Half-Life Uplink",
            "valve",
            () => new[]
                {
                    "hldemo1",
                    "hldemo2",
                    "hldemo3"
                }
            .Select(n => new MapInfo(n, MapCategory.Campaign))
            .ToImmutableDictionary(m => m.Name));

        /// <summary>Team Fortress Classic.</summary>
        public static GameInfo TeamFortressClassic { get; } = new GameInfo(
            GameEngine.GoldSource,
            "Team Fortress Classic",
            "tfc",
            () => new[]
                {
                    "2fort",
                    "avanti",
                    "badlands",
                    "casbah",
                    "crossover2",
                    "cz2",
                    "dustbowl",
                    "epicenter",
                    "flagrun",
                    "hunted",
                    "push",
                    "ravelin",
                    "rock2",
                    "warpath",
                    "well"
                }
            .Select(n => new MapInfo(n, MapCategory.Multiplayer))
            .ToImmutableDictionary(m => m.Name));

        /// <summary>Day of Defeat.</summary>
        public static GameInfo DayOfDefeat { get; } = new GameInfo(
            GameEngine.GoldSource,
            "Day of Defeat",
            "dod",
            () => new[]
                {
                    "dod_anzio",
                    "dod_avalanche",
                    "dod_caen",
                    "dod_charlie",
                    "dod_chemille",
                    "dod_donner",
                    "dod_escape",
                    "dod_falaise",
                    "dod_flash",
                    "dod_flugplatz",
                    "dod_forest",
                    "dod_glider",
                    "dod_jagd",
                    "dod_kalt",
                    "dod_kraftstoff",
                    "dod_merderet",
                    "dod_northbound",
                    "dod_saints",
                    "dod_sturm",
                    "dod_switch",
                    "dod_vicenza",
                    "dod_zalec"
                }
            .Select(n => new MapInfo(n, MapCategory.Multiplayer))
            .ToImmutableDictionary(m => m.Name));

        /// <summary>Ricochet.</summary>
        public static GameInfo Ricochet { get; } = new GameInfo(
            GameEngine.GoldSource,
            "Ricochet",
            "ricochet",
            () => new[]
                {
                    "rc_arena",
                    "rc_deathmatch",
                    "rc_deathmatch2"
                }
            .Select(n => new MapInfo(n, MapCategory.Multiplayer))
            .ToImmutableDictionary(m => m.Name));

        /// <summary>Counter-Strike.</summary>
        public static GameInfo CounterStrike { get; } = new GameInfo(
            GameEngine.GoldSource,
            "Counter-Strike",
            "cstrike",
            () => new[]
                {
                    "as_oilrig",
                    "cs_747",
                    "cs_assault",
                    "cs_backalley",
                    "cs_estate",
                    "cs_havana",
                    "cs_italy",
                    "cs_militia",
                    "cs_office",
                    "cs_siege",
                    "de_airstrip",
                    "de_aztec",
                    "de_cbble",
                    "de_chateau",
                    "de_dust",
                    "de_dust2",
                    "de_inferno",
                    "de_nuke",
                    "de_piranesi",
                    "de_prodigy",
                    "de_storm",
                    "de_survivor",
                    "de_torn",
                    "de_train",
                    "de_vertigo"
                }
            .Select(n => new MapInfo(n, MapCategory.Multiplayer))
            .ToImmutableDictionary(m => m.Name));

        /// <summary>Counter-Strike: Condition Zero.</summary>
        public static GameInfo ConditionZero { get; } = new GameInfo(
            GameEngine.GoldSource,
            "Counter-Strike: Condition Zero",
            "czero",
            () => new[]
                {
                    "cs_downed_cz",
                    "cs_havana_cz",
                    "cs_italy_cz",
                    "cs_militia_cz",
                    "cs_office_cz",
                    "de_airstrip_cz",
                    "de_aztec_cz",
                    "de_cbble_cz",
                    "de_chateau_cz",
                    "de_corruption_cz",
                    "de_dust_cz",
                    "de_dust2_cz",
                    "de_fastline_cz",
                    "de_inferno_cz",
                    "de_piranesi_cz",
                    "de_prodigy_cz",
                    "de_sienna_cz",
                    "de_stadium_cz",
                    "de_tides_cz",
                    "de_torn_cz",
                    "de_truth_cz",
                    "de_vostok_cz"
                }
            .Select(n => new MapInfo(n, MapCategory.Multiplayer))
            .ToImmutableDictionary(m => m.Name));

        /// <summary>Counter-Strike: Condition Zero Deleted Scenes.</summary>
        public static GameInfo ConditionZeroDeletedScenes { get; } = new GameInfo(
            GameEngine.GoldSource,
            "Counter-Strike: Condition Zero Deleted Scenes",
            "czeror",
            () => new[]
                {
                    "cz_alamo",
                    "cz_alamo2",
                    "cz_brecon01",
                    "cz_brecon02",
                    "cz_brecon03",
                    "cz_brecon04",
                    "cz_brecon05",
                    "cz_brecon06",
                    "cz_downed1",
                    "cz_downed2",
                    "cz_downed3",
                    "cz_downed4",
                    "cz_druglab1",
                    "cz_druglab2",
                    "cz_druglab3",
                    "cz_druglab4",
                    "cz_end",
                    "cz_fastline1",
                    "cz_fastline2",
                    "cz_fastline3",
                    "cz_fastline4",
                    "cz_hankagai1",
                    "cz_hankagai2",
                    "cz_hankagai3",
                    "cz_hankagai4",
                    "cz_hankagai5",
                    "cz_hr01",
                    "cz_hr02",
                    "cz_hr02b",
                    "cz_hr03",
                    "cz_hr04",
                    "cz_hr05",
                    "cz_hr06",
                    "cz_hr07",
                    "cz_lostcause",
                    "cz_lostcause2",
                    "cz_miami1",
                    "cz_miami2",
                    "cz_motor1",
                    "cz_motor2",
                    "cz_motor3",
                    "cz_pipedream1",
                    "cz_pipedream2",
                    "cz_pipedream3",
                    "cz_recoil-intro",
                    "cz_recoil",
                    "cz_recoil2",
                    "cz_run1",
                    "cz_run2",
                    "cz_run3",
                    "cz_sandstorm",
                    "cz_sandstorm2",
                    "cz_sandstorm3",
                    "cz_sandstorm4",
                    "cz_silo01",
                    "cz_silo02",
                    "cz_silo03",
                    "cz_silo04",
                    "cz_silo05",
                    "cz_thinice01",
                    "cz_thinice02",
                    "cz_thinice03",
                    "cz_train01",
                    "cz_train02",
                    "cz_train03",
                    "cz_truth1",
                    "cz_truth2",
                    "cz_truth3",
                    "cz_truth4",
                    "cz_turncrank",
                    "cz_turncrank2",
                    "cz_turncrank3",
                    "cz_worldmap"
                }
            .Select(n => new MapInfo(n, MapCategory.Campaign))
            .ToImmutableDictionary(m => m.Name));

        /// <summary>
        /// List of all Valve-made Half-Life 1 games and expansion packs running on the GoldSource engine
        /// installable through Steam supported by this tool.
        /// </summary>
        public static ImmutableArray<GameInfo> HalfLifeGames { get; } = ImmutableArray.Create(
            HalfLife1,
            OpposingForce,
            BlueShift
            );

        /// <summary>
        /// List of all Valve-made Counter-Strike games running on the GoldSource engine
        /// installable through Steam supported by this tool.
        /// </summary>
        public static ImmutableArray<GameInfo> CounterStrikeGames { get; } = ImmutableArray.Create(
            CounterStrike,
            ConditionZero,
            ConditionZeroDeletedScenes
            );

        /// <summary>
        /// List of all Valve-made games running on the GoldSource engine
        /// installable through Steam supported by this tool.
        /// </summary>
        public static ImmutableArray<GameInfo> GoldSourceGames { get; } = ImmutableArray.CreateRange(
            HalfLifeGames
                .Concat(CounterStrikeGames)
                .Append(TeamFortressClassic)
                .Append(DayOfDefeat)
                .Append(Ricochet)
            );

        /// <summary>List of all Valve-made games installable through Steam supported by this tool.</summary>
        public static ImmutableArray<GameInfo> Games => GoldSourceGames;

        /// <summary>Returns whether the given map name is a map in any Valve game.</summary>
        public static bool IsMap(string value) => Games.Any(g => g.IsMap(value));

        /// <summary>Returns whether the given map name is a map of the given category in any Valve game.</summary>
        public static bool IsMap(string value, MapCategory category) => Games.Any(g => g.IsMap(value, category));

        /// <summary>Returns whether the given map name is a campaign map in any Valve game.</summary>
        public static bool IsCampaignMap(string value) => IsMap(value, MapCategory.Campaign);

        /// <summary>Returns whether the given map name is a training map in any Valve game.</summary>
        public static bool IsTrainingMap(string value) => IsMap(value, MapCategory.Training);

        /// <summary>Returns whether the given map name is a multiplayer map in any Valve game.</summary>
        public static bool IsMultiplayerMap(string value) => IsMap(value, MapCategory.Multiplayer);
    }
}
