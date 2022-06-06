using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;
using System.Collections.Immutable;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Reworks how music is played to use <c>ambient_music</c> instead.
    /// </summary>
    internal sealed class ReworkMusicPlaybackUpgrade : IMapUpgradeAction
    {
        private const string TriggerOnceClassName = "trigger_once";
        private const string AmbientMusicClassName = "ambient_music";
        private const string FileNameKey = "filename";
        private const string CommandKey = "command";
        private const string TargetSelectorKey = "target_selector";
        private const string RemoveOnFireKey = "remove_on_fire";

        private enum AmbientMusicCommand
        {
            Play = 0,
            Loop,
            Pause,
            Resume,
            Fadeout,
            Stop
        }

        private enum AmbientMusicTargetSelector
        {
            AllPlayers = 0,
            Activator,
            LocalPlayer,
            Radius
        }

        private const int StopTrackId = -1;

        private static readonly ImmutableDictionary<int, string> TrackToNameMap = new Dictionary<int, string>
        {
            { 0, "" },
            { 1, "" },
            { 2, "media/Half-Life01.mp3" },
            { 3, "media/Prospero01.mp3" },
            { 4, "media/Half-Life12.mp3" },
            { 5, "media/Half-Life07.mp3" },
            { 6, "media/Half-Life10.mp3" },
            { 7, "media/Suspense01.mp3" },
            { 8, "media/Suspense03.mp3" },
            { 9, "media/Half-Life09.mp3" },
            { 10, "media/Half-Life02.mp3" },
            { 11, "media/Half-Life13.mp3" },
            { 12, "media/Half-Life04.mp3" },
            { 13, "media/Half-Life15.mp3" },
            { 14, "media/Half-Life14.mp3" },
            { 15, "media/Half-Life16.mp3" },
            { 16, "media/Suspense02.mp3" },
            { 17, "media/Half-Life03.mp3" },
            { 18, "media/Half-Life08.mp3" },
            { 19, "media/Prospero02.mp3" },
            { 20, "media/Half-Life05.mp3" },
            { 21, "media/Prospero04.mp3" },
            { 22, "media/Half-Life11.mp3" },
            { 23, "media/Half-Life06.mp3" },
            { 24, "media/Prospero03.mp3" },
            { 25, "media/Half-Life17.mp3" },
            { 26, "media/Prospero06.mp3" },
            { 27, "media/Suspense05.mp3" },
            { 28, "media/Suspense07.mp3" }
        }.ToImmutableDictionary();

        public void Apply(MapUpgradeContext context)
        {
            //Remap to ambient_music.
            foreach (var target in context.Map.Entities.OfClass("target_cdaudio"))
            {
                target.ClassName = AmbientMusicClassName;

                UpdateEntities(target, target, AmbientMusicTargetSelector.Radius);
            }

            //Remap to trigger_once triggering ambient_music.
            foreach (var trigger in context.Map.Entities.OfClass("trigger_cdaudio").ToList())
            {
                trigger.ClassName = TriggerOnceClassName;

                var music = context.Map.Entities.CreateNewEntity(AmbientMusicClassName);

                UpdateEntities(trigger, music, AmbientMusicTargetSelector.LocalPlayer);
            }

            var worldspawn = context.Map.Entities.Worldspawn;

            var track = worldspawn.GetInteger("sounds", 0);
            worldspawn.Remove("sounds");

            //These two tracks are empty names and will just interfere with other music playback.
            if (track != 0 && track != 1)
            {
                var music = context.Map.Entities.CreateNewEntity(AmbientMusicClassName);

                if (track == StopTrackId)
                {
                    music.SetInteger(CommandKey, (int)AmbientMusicCommand.Stop);
                }
                else
                {
                    music.SetInteger(CommandKey, (int)AmbientMusicCommand.Play);
                    music.SetString(FileNameKey, GetFileName(track));
                }

                music.SetInteger(TargetSelectorKey, (int)AmbientMusicTargetSelector.LocalPlayer);
                music.SetInteger(RemoveOnFireKey, 1);

                music.SetTargetName("game_playeractivate");
            }
        }

        private static void UpdateEntities(Entity trigger, Entity music, AmbientMusicTargetSelector targetSelector)
        {
            var track = trigger.GetInteger("health", 0);
            trigger.Remove("health");

            if (track == StopTrackId)
            {
                music.SetInteger(CommandKey, (int)AmbientMusicCommand.Stop);
                trigger.Remove(FileNameKey);
            }
            else
            {
                music.SetInteger(CommandKey, (int)AmbientMusicCommand.Play);
                music.SetString(FileNameKey, GetFileName(track));
            }

            music.SetInteger(TargetSelectorKey, (int)targetSelector);
            music.SetInteger(RemoveOnFireKey, 1);
        }

        private static string GetFileName(int track)
        {
            if (!TrackToNameMap.TryGetValue(track, out var fileName))
            {
                //TODO: log a warning.
                fileName = "";
            }

            return fileName;
        }
    }
}
