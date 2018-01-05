using GrandTheftSpace.CoreGame.Gameplay;
using GrandTheftSpace.CoreGame.Serialization.Space;
using GrandTheftSpace.CoreGame.UserInterface;
using GTA;
using GTA.Native;
using GTAMenu;
using System;

namespace GrandTheftSpace.CoreGame.ScriptUtilities
{
    internal class LevelManager : ScriptUtility
    {
        private int screenFadeTime = 1000;

        public LevelManager(Script script, MenuManager menuManager) : base(script)
        {
            script.Tick += OnTick;
            script.Aborted += OnAborted;

            MenuManager = menuManager;
            MenuManager.LevelEditorMenu.OpenFileMenu.MainMenu.ItemSelected += OnFileSelected;
        }

        /// <summary>
        /// Contiains the mods menus.
        /// </summary>
        public MenuManager MenuManager { get; private set; }

        /// <summary>
        /// The current level.
        /// </summary>
        public RuntimeLevel Level { get; private set; }

        /// <summary>
        /// Returns true if the level has loaded.
        /// </summary>
        public bool IsLevelLoaded { get; private set; }

        #region Menu Events

        private void OnFileSelected(object sender, NativeMenuItemEventArgs e)
        {
            if (e.MenuItem == null)
            {
                return;
            }

            var tag = (SpaceLevel)e.MenuItem.Tag;

            if (tag == null)
            {
                return;
            }

            Level = new RuntimeLevel(tag);
        }

        #endregion

        #region Script Events

        private void OnTick(object sender, EventArgs e)
        {
            BeginLevelLoad();
            UpdateLevel();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            ResetScreenFade();
            UnloadLevel(true);

            // DEBUG
            Game.Player.Character.FreezePosition = false;
        }

        #endregion

        public override void ReadSettings(ScriptSettings scriptSettings)
        {
            screenFadeTime = scriptSettings.GetValue("levels", "screen_fade_time", screenFadeTime);
            scriptSettings.SetValue("levels", "screen_fade_time", screenFadeTime);
        }

        private void BeginLevelLoad()
        {
            if (Level == null)
            {
                if (IsLevelLoaded)
                {
                    IsLevelLoaded = false;
                }
                return;
            }

            if (!IsLevelLoaded)
            {
                if (!Game.IsScreenFadingOut)
                {
                    Game.FadeScreenOut(screenFadeTime);
                }

                if (Game.IsScreenFadedOut)
                {
                    LoadLevel(Level);

                    IsLevelLoaded = true;

                    Game.FadeScreenIn(screenFadeTime);

                    // DEBUGGING
                    Game.Player.Character.Position = Level.LevelMetadata.Position;
                    Game.Player.Character.FreezePosition = true;
                }
            }
        }

        private void UpdateLevel()
        {
            if (Level == null || !IsLevelLoaded)
            {
                return;
            }

            Level.Tick();
        }

        private void LoadLevel(RuntimeLevel level)
        {
            level.Init();
        }

        private void UnloadLevel(bool aborted)
        {
            if (Level == null)
            {
                return;
            }

            Level.Stop();

            if (aborted)
            {
                Level.Abort();
            }

            IsLevelLoaded = false;

            Level = null;
        }

        private void ResetScreenFade()
        {
            if (Game.IsScreenFadedOut || Game.IsScreenFadedOut)
            {
                if (!Game.Player.Character.IsDead)
                {
                    Game.FadeScreenIn(0);
                }
            }
        }
    }
}
