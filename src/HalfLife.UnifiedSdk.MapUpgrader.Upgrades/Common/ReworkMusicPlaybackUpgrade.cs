using HalfLife.UnifiedSdk.Utilities.Entities;
using HalfLife.UnifiedSdk.Utilities.Games;
using HalfLife.UnifiedSdk.Utilities.Tools;
using HalfLife.UnifiedSdk.Utilities.Tools.UpgradeTool;

namespace HalfLife.UnifiedSdk.MapUpgrader.Upgrades.Common
{
    /// <summary>
    /// Reworks how music is played to use <c>ambient_music</c> instead.
    /// </summary>
    internal sealed class ReworkMusicPlaybackUpgrade : MapUpgrade
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
            Fadeout,
            Stop
        }

        private enum AmbientMusicTargetSelector
        {
            AllPlayers = 0,
            Activator,
            Radius
        }

        protected override void ApplyCore(MapUpgradeContext context)
        {
            //Remap to ambient_music.
            foreach (var target in context.Map.Entities.OfClass("target_cdaudio"))
            {
                target.ClassName = AmbientMusicClassName;

                UpdateEntities(target, target, AmbientMusicTargetSelector.Radius, context.GameInfo);
            }

            //Remap to trigger_once triggering ambient_music.
            foreach (var trigger in context.Map.Entities.OfClass("trigger_cdaudio").ToList())
            {
                Entity music;

                //If the entity has a targetname then we'll assume it will be triggered instead of touched.
                if (!string.IsNullOrWhiteSpace(trigger.GetTargetName()))
                {
                    trigger.ClassName = AmbientMusicClassName;
                    music = trigger;
                    //Remove the brush model so it doesn't interfere.
                    trigger.Remove(KeyValueUtilities.Model);
                }
                else
                {
                    trigger.ClassName = TriggerOnceClassName;
                    music = context.Map.Entities.CreateNewEntity(AmbientMusicClassName);

                    var targetName = context.Map.Entities.GenerateUniqueTargetName("music");

                    music.SetTargetName(targetName);
                    trigger.SetTarget(targetName);
                }

                UpdateEntities(trigger, music, AmbientMusicTargetSelector.AllPlayers, context.GameInfo);
            }

            var worldspawn = context.Map.Entities.Worldspawn;

            var track = worldspawn.GetInteger("sounds", 0);
            worldspawn.Remove("sounds");

            //These two tracks are empty names and will just interfere with other music playback.
            if (track != 0 && track != 1)
            {
                var music = context.Map.Entities.CreateNewEntity(AmbientMusicClassName);

                if (track == GameMedia.StopTrackId)
                {
                    music.SetInteger(CommandKey, (int)AmbientMusicCommand.Stop);
                }
                else
                {
                    music.SetInteger(CommandKey, (int)AmbientMusicCommand.Play);
                    music.SetString(FileNameKey, GetFileName(track, context.GameInfo));
                }

                music.SetInteger(TargetSelectorKey, (int)AmbientMusicTargetSelector.AllPlayers);
                music.SetInteger(RemoveOnFireKey, 1);

                music.SetTargetName("game_playeractivate");
            }
        }

        private static void UpdateEntities(Entity trigger, Entity music, AmbientMusicTargetSelector targetSelector, GameInfo gameInfo)
        {
            var track = trigger.GetInteger("health", 0);
            trigger.Remove("health");

            if (track == GameMedia.StopTrackId)
            {
                music.SetInteger(CommandKey, (int)AmbientMusicCommand.Stop);
                trigger.Remove(FileNameKey);
            }
            else
            {
                music.SetInteger(CommandKey, (int)AmbientMusicCommand.Play);
                music.SetString(FileNameKey, GetFileName(track, gameInfo));
            }

            music.SetInteger(TargetSelectorKey, (int)targetSelector);
            music.SetInteger(RemoveOnFireKey, 1);
        }

        private static string GetFileName(int track, GameInfo gameInfo)
        {
            if (GameMedia.TrackToMusicMap.TryGetValue(track, out var fileName))
            {
                //Point filename to game-specific soundtrack.
                if (gameInfo.MusicFileNames.Contains(fileName)
                    && GameMedia.GameToMusicPrefixMap.TryGetValue(gameInfo.ModDirectory, out var prefix))
                {
                    fileName = GameMedia.GetGameSpecificMusicName(fileName, prefix);
                }

                //Make sure paths use correct format in the BSP file.
                return GameMedia.GetMusicFileName(fileName).Replace("\\", "/");
            }

            //TODO: log a warning.
            return "";
        }
    }
}
