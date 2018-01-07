using GrandTheftSpace.CoreGame.Gameplay;
using GrandTheftSpace.CoreGame.ScriptUtilities.LevelManagerUtilities;
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
        private int screenFadeTime = 1000; // The duration of the screen fade between levels.
        private RuntimeLevel currentLevel; // The current level.
        private RuntimeLevel initialLevel; // The last level that was loaded.

        public LevelManager(Script script, MenuManager menuManager) : base(script)
        {
            script.Tick += OnTick;
            script.Aborted += OnAborted;

            MenuManager = menuManager;
            SpaceLevelEditor = new SpaceLevelEditor(menuManager);

            SubscribeToMenuEvents();
        }

        /// <summary>
        /// Contiains the mods menus.
        /// </summary>
        public MenuManager MenuManager { get; private set; }

        /// <summary>
        /// The current level.
        /// </summary>
        public RuntimeLevel Level {
            get {
                return currentLevel;
            }
            set {
                if (IsLevelLoaded || currentLevel == null)
                {
                    if (SpaceLevelEditor.Activate)
                    {
                        SpaceLevelEditor.Stop();
                    }

                    initialLevel = currentLevel;
                    currentLevel = value;
                    IsLevelLoaded = false;
                }
            }
        }

        /// <summary>
        /// The current space level editor.
        /// </summary>
        public SpaceLevelEditor SpaceLevelEditor { get; private set; }

        /// <summary>
        /// Returns true if the level has loaded.
        /// </summary>
        public bool IsLevelLoaded { get; private set; }

        #region Menu Events

        private void SubscribeToMenuEvents()
        {
            MenuManager.LevelSelectionMenu.OpenFileMenu.MainMenu.ItemSelected += OnFileSelected;
        }

        private void OnFileSelected(object sender, NativeMenuItemEventArgs e)
        {
            if (e.MenuItem == null)
            {
                return;
            }

            var levelMetadata = (SpaceLevel)e.MenuItem.Tag;

            if (levelMetadata == null)
            {
                return;
            }

            Level = new RuntimeLevel(levelMetadata);

            SpaceLevelEditor.BeginEdit(Level);
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
            StopLevelEditor(true);
            UnloadLevel(true);

            // DEBUG
            Game.Player.Character.FreezePosition = false;
        }

        #endregion

        #region ScriptUtility

        public override void ReadSettings(ScriptSettings scriptSettings)
        {
            screenFadeTime = scriptSettings.GetValue("levels", "screen_fade_time", screenFadeTime);
            scriptSettings.SetValue("levels", "screen_fade_time", screenFadeTime);
            SpaceLevelEditor.ReadSettings(scriptSettings);
        }

        #endregion

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
                    // DEBUGGING
                    Game.Player.Character.Position = Level.LevelMetadata.Position;
                    Game.Player.Character.FreezePosition = true;

                    if (initialLevel != null)
                    {
                        initialLevel.Stop();

                        initialLevel = null;
                    }

                    LoadLevel(Level);

                    IsLevelLoaded = true;

                    if (SpaceLevelEditor.Activate)
                    {
                        SpaceLevelEditor.Init();
                    }

                    Game.FadeScreenIn(screenFadeTime);
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

            UpdateLevelEditor();
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

        private void UpdateLevelEditor()
        {
            if (SpaceLevelEditor.Activate)
            {
                SpaceLevelEditor.Tick();
            }
        }

        private void StopLevelEditor(bool aborted)
        {
            SpaceLevelEditor.Stop();

            if (aborted)
            {
                SpaceLevelEditor.Abort();
            }
        }
    }
}
