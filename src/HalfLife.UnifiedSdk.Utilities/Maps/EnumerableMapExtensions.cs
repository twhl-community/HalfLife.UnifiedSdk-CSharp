using HalfLife.UnifiedSdk.Utilities.Games;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfLife.UnifiedSdk.Utilities.Maps
{
    /// <summary>Extensions for <see cref="IEnumerable{T}"/> of <see cref="Map"/>.</summary>
    public static class EnumerableMapExtensions
    {
        /// <summary>Filters a sequence of maps to include only those that are Valve-made maps.</summary>
        /// <seealso cref="ValveGames.IsMap(string)"/>
        /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Map> WhereIsMap(this IEnumerable<Map> maps)
        {
            ArgumentNullException.ThrowIfNull(maps);
            return maps.Where(m => ValveGames.IsMap(m.BaseName));
        }

        /// <summary>Filters a sequence of maps to include only those that are Valve-made maps of <paramref name="category"/>.</summary>
        /// <seealso cref="ValveGames.IsMap(string, MapCategory)"/>
        /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Map> WhereIsMap(this IEnumerable<Map> maps, MapCategory category)
        {
            ArgumentNullException.ThrowIfNull(maps);
            return maps.Where(m => ValveGames.IsMap(m.BaseName, category));
        }

        /// <summary>Filters a sequence of maps to include only those that are Valve-made campaign maps.</summary>
        /// <seealso cref="ValveGames.IsCampaignMap(string)"/>
        /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Map> WhereIsCampaignMap(this IEnumerable<Map> maps)
        {
            ArgumentNullException.ThrowIfNull(maps);
            return maps.Where(m => ValveGames.IsCampaignMap(m.BaseName));
        }

        /// <summary>Filters a sequence of maps to include only those that are Valve-made campaign maps from <paramref name="game"/>.</summary>
        /// <seealso cref="GameInfo.IsCampaignMap(string)"/>
        /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Map> WhereIsCampaignMap(this IEnumerable<Map> maps, GameInfo game)
        {
            ArgumentNullException.ThrowIfNull(maps);
            return maps.Where(m => game.IsCampaignMap(m.BaseName));
        }

        /// <summary>Filters a sequence of maps to include only those that are Valve-made training maps.</summary>
        /// <seealso cref="ValveGames.IsTrainingMap(string)"/>
        /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Map> WhereIsTrainingMap(this IEnumerable<Map> maps)
        {
            ArgumentNullException.ThrowIfNull(maps);
            return maps.Where(m => ValveGames.IsTrainingMap(m.BaseName));
        }

        /// <summary>Filters a sequence of maps to include only those that are Valve-made training maps from <paramref name="game"/>.</summary>
        /// <seealso cref="GameInfo.IsTrainingMap(string)"/>
        /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Map> WhereIsTrainingMap(this IEnumerable<Map> maps, GameInfo game)
        {
            ArgumentNullException.ThrowIfNull(maps);
            return maps.Where(m => game.IsTrainingMap(m.BaseName));
        }

        /// <summary>Filters a sequence of maps to include only those that are Valve-made multiplayer maps.</summary>
        /// <seealso cref="ValveGames.IsMultiplayerMap(string)"/>
        /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Map> WhereIsMultiplayerMap(this IEnumerable<Map> maps)
        {
            ArgumentNullException.ThrowIfNull(maps);
            return maps.Where(m => ValveGames.IsMultiplayerMap(m.BaseName));
        }

        /// <summary>Filters a sequence of maps to include only those that are Valve-made multiplayer maps from <paramref name="game"/>.</summary>
        /// <seealso cref="GameInfo.IsMultiplayerMap(string)"/>
        /// <exception cref="ArgumentNullException"><paramref name="maps"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Map> WhereIsMultiplayerMap(this IEnumerable<Map> maps, GameInfo game)
        {
            ArgumentNullException.ThrowIfNull(maps);
            return maps.Where(m => game.IsMultiplayerMap(m.BaseName));
        }
    }
}
