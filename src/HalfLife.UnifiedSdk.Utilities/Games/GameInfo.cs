using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Games
{
    /// <summary>
    /// Provides information about a game, such as the engine it is running on and which maps it has.
    /// </summary>
    public class GameInfo
    {
        private readonly Lazy<ImmutableDictionary<string, MapInfo>> _maps;

        /// <summary>Which engine this game is running on.</summary>
        public GameEngine Engine { get; }

        /// <summary>Name of the game, for printing.</summary>
        public string Name { get; }

        /// <summary>
        /// The name of the mod directory.
        /// For GoldSource engine games this is the directory located in the Half-Life directory.
        /// For Source engine games this is the directory located in the sourcemods directory.
        /// Source 2 doesn't have mod support at present, so this value should be empty for that engine.
        /// </summary>
        public string ModDirectory { get; }

        /// <summary>Dictionary of all official maps in this game.</summary>
        public ImmutableDictionary<string, MapInfo> Maps => _maps.Value;

        /// <summary>Gets an enumerable collection of all of the campaign maps.</summary>
        public IEnumerable<MapInfo> CampaignMaps => GetMaps(MapCategory.Campaign);

        /// <summary>Gets an enumerable collection of all of the training maps.</summary>
        public IEnumerable<MapInfo> TrainingMaps => GetMaps(MapCategory.Training);

        /// <summary>Gets an enumerable collection of all of the multiplayer maps.</summary>
        public IEnumerable<MapInfo> MultiplayerMaps => GetMaps(MapCategory.Multiplayer);

        // Public so scripts can create instances and use them in existing APIs.
        /// <summary>
        /// Creates a new game info object.
        /// </summary>
        /// <param name="engine"><see cref="Engine"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="modDirectory"><see cref="ModDirectory"/></param>
        /// <param name="maps">A function that returns a dictionary of map info. Will be invoked once if and when map info is requested.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/>, <paramref name="modDirectory"/> or <paramref name="maps"/> are <see langword="null"/>.
        /// </exception>
        public GameInfo(GameEngine engine, string name, string modDirectory, Func<ImmutableDictionary<string, MapInfo>> maps)
        {
            Engine = engine;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ModDirectory = modDirectory ?? throw new ArgumentNullException(nameof(modDirectory));
            _maps = new(maps ?? throw new ArgumentNullException(nameof(maps)));
        }

        /// <summary>Gets an enumerable collection of all of the maps of the given category.</summary>
        public IEnumerable<MapInfo> GetMaps(MapCategory category) => Maps.Values.Where(m => m.Category == category);

        /// <summary>Returns whether the given map name is a map in this game.</summary>
        public bool IsMap(string value) => Maps.ContainsKey(value);

        /// <summary>Returns whether the given map name is a map of the given category in this game.</summary>
        public bool IsMap(string value, MapCategory category) => Maps.TryGetValue(value, out var info) && info.Category == category;

        /// <summary>Returns whether the given map name is a campaign map in this game.</summary>
        public bool IsCampaignMap(string value) => IsMap(value, MapCategory.Campaign);

        /// <summary>Returns whether the given map name is a training map in this game.</summary>
        public bool IsTrainingMap(string value) => IsMap(value, MapCategory.Training);

        /// <summary>Returns whether the given map name is a multiplayer map in this game.</summary>
        public bool IsMultiplayerMap(string value) => IsMap(value, MapCategory.Multiplayer);
    }
}
