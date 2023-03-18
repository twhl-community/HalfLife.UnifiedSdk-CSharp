using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace HalfLife.UnifiedSdk.Utilities.Games
{
    /// <summary>
    /// Contains information about the contents of the <c>media</c> directory.
    /// </summary>
    public static class GameMedia
    {
        /// <summary> The directory name. </summary>
        public const string MediaDirectory = "media";

        /// <summary> The music file extension. </summary>
        public const string MusicExtension = ".mp3";

        /// <summary> The prefix used for some of the music. </summary>
        public const string HalfLifeMusicPrefix = "Half-Life";

        /// <summary>
        /// The prefix used for Opposing Force music (only in Unified SDK).
        /// </summary>
        public const string OpposingForceMusicPrefix = "Opposing-Force";

        /// <summary>
        /// The prefix used for Blue Shift music (only in Unified SDK).
        /// </summary>
        public const string BlueShiftMusicPrefix = "Blue-Shift";

        /// <summary> Map of mod directory to music prefix (only in Unified SDK). </summary>
        public static readonly ImmutableDictionary<string, string> GameToMusicPrefixMap = new Dictionary<string, string>
        {
            { ValveGames.OpposingForce.ModDirectory, OpposingForceMusicPrefix },
            { ValveGames.BlueShift.ModDirectory, BlueShiftMusicPrefix }
        }.ToImmutableDictionary();

        /// <summary>
        /// List of music filenames.
        /// </summary>
        public static readonly ImmutableList<string> MusicFileNames = ImmutableList.Create(
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
            );

        /// <summary> The track id used to signal that music should stop playing. </summary>
        public const int StopTrackId = -1;

        /// <summary> Map of track ids to music filenames. </summary>
        public static readonly ImmutableDictionary<int, string> TrackToMusicMap = new Dictionary<int, string>
        {
            { 0, "" },
            { 1, "" },
            { 2, "Half-Life01" },
            { 3, "Prospero01" },
            { 4, "Half-Life12" },
            { 5, "Half-Life07" },
            { 6, "Half-Life10" },
            { 7, "Suspense01" },
            { 8, "Suspense03" },
            { 9, "Half-Life09" },
            { 10, "Half-Life02" },
            { 11, "Half-Life13" },
            { 12, "Half-Life04" },
            { 13, "Half-Life15" },
            { 14, "Half-Life14" },
            { 15, "Half-Life16" },
            { 16, "Suspense02" },
            { 17, "Half-Life03" },
            { 18, "Half-Life08" },
            { 19, "Prospero02" },
            { 20, "Half-Life05" },
            { 21, "Prospero04" },
            { 22, "Half-Life11" },
            { 23, "Half-Life06" },
            { 24, "Prospero03" },
            { 25, "Half-Life17" },
            { 26, "Prospero05" },
            { 27, "Suspense05" },
            { 28, "Suspense07" }
        }.ToImmutableDictionary();

        /// <summary>
        /// Given a music filename and a game name, returns a filename suitable for the given game.
        /// </summary>
        /// <remarks>
        /// Some music names contain <c>Half-Life</c> in the name whilst others are named generically.
        /// This provides a name that accounts for the differences in naming style.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="musicFileName"/> or <paramref name="gameName"/> are <see langword="null"/>.
        /// </exception>
        public static string GetGameSpecificMusicName(string musicFileName, string gameName)
        {
            ArgumentNullException.ThrowIfNull(musicFileName);
            ArgumentNullException.ThrowIfNull(gameName);

            if (musicFileName.StartsWith(HalfLifeMusicPrefix))
            {
                return musicFileName.Replace(HalfLifeMusicPrefix, gameName);
            }
            else
            {
                return gameName + musicFileName;
            }
        }

        /// <summary>Gets the relative path to the music file with the given base name.</summary>
        public static string GetMusicFileName(string baseName)
        {
            ArgumentNullException.ThrowIfNull(baseName);

            return Path.Combine(MediaDirectory, baseName + MusicExtension);
        }
    }
}
