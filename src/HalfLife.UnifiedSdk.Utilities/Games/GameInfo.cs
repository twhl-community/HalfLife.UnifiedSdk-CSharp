using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Games
{
    /// <summary>
    /// Provides information about a game, such as the engine it is running on and which maps it has.
    /// </summary>
    public class GameInfo : IEquatable<GameInfo?>
    {
        private static GameInfo CreateGenericGameInfo(GameEngine engine)
        {
            return new(engine, "generic", string.Empty, () => ImmutableDictionary<string, MapInfo>.Empty, () => ImmutableHashSet<string>.Empty);
        }

        /// <summary>Generic GoldSource engine game info.</summary>
        public static GameInfo GenericGoldSourceGame { get; } = CreateGenericGameInfo(GameEngine.GoldSource);

        /// <summary>Generic Source engine game info.</summary>
        public static GameInfo GenericSourceGame { get; } = CreateGenericGameInfo(GameEngine.Source);

        /// <summary>Generic Source 2 engine game info.</summary>
        public static GameInfo GenericSource2Game { get; } = CreateGenericGameInfo(GameEngine.Source2);

        private readonly Lazy<ImmutableDictionary<string, MapInfo>> _maps;

        private readonly Lazy<ImmutableHashSet<string>> _musicFileNames;

        /// <summary>Which engine this game is running on.</summary>
        public GameEngine Engine { get; }

        /// <summary>Name of the game, for printing.</summary>
        public string Name { get; }

        /// <summary>
        /// The name of the mod directory.
        /// For GoldSource engine games this is the directory located in the Half-Life directory.
        /// For Source engine games this is the directory located in the sourcemods directory.
        /// Source 2 doesn't have mod support at present, so this value should be empty for that engine.
        /// Will be empty if this is a generic game.
        /// </summary>
        public string ModDirectory { get; }

        /// <summary>Dictionary of all official maps in this game.</summary>
        public ImmutableDictionary<string, MapInfo> Maps => _maps.Value;

        /// <summary>Set of all music filenames used by this game.</summary>
        public ImmutableHashSet<string> MusicFileNames => _musicFileNames.Value;

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
        /// <param name="musicFileNames">
        /// A function that returns a set of music filenames used by this game.
        /// Will be invoked once if and when music info is requested.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/>, <paramref name="modDirectory"/>,
        /// <paramref name="maps"/> or <paramref name="musicFileNames"/> are <see langword="null"/>.
        /// </exception>
        public GameInfo(GameEngine engine, string name, string modDirectory,
            Func<ImmutableDictionary<string, MapInfo>> maps, Func<ImmutableHashSet<string>> musicFileNames)
        {
            Engine = engine;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ModDirectory = modDirectory ?? throw new ArgumentNullException(nameof(modDirectory));
            _maps = new(maps ?? throw new ArgumentNullException(nameof(maps)));
            _musicFileNames = new(musicFileNames ?? throw new ArgumentNullException(nameof(maps)));
        }

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
        public GameInfo(GameEngine engine, string name, string modDirectory,
            Func<ImmutableDictionary<string, MapInfo>> maps)
            : this(engine, name, modDirectory, maps, () => ImmutableHashSet<string>.Empty)
        {
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

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as GameInfo);
        }

        /// <inheritdoc/>
        public bool Equals(GameInfo? other)
        {
            return other is not null &&
                   Engine == other.Engine &&
                   ModDirectory == other.ModDirectory;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Engine, ModDirectory);
        }

        /// <summary>
        /// Indicates whether <paramref name="left"/> is equal to <paramref name="right"/>.
        /// </summary>
        public static bool operator ==(GameInfo? left, GameInfo? right)
        {
            return EqualityComparer<GameInfo>.Default.Equals(left, right);
        }

        /// <summary>
        /// Indicates whether <paramref name="left"/> is not equal to <paramref name="right"/>.
        /// </summary>
        public static bool operator !=(GameInfo? left, GameInfo? right)
        {
            return !(left == right);
        }
    }
}
