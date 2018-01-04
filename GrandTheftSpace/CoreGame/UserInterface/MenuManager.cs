using GrandTheftSpace.CoreGame.ScriptThreads;
using GrandTheftSpace.CoreGame.UserInterface.MenuExpansions;
using GTA;
using GTAMenu;
using System;
using System.Windows.Forms;

namespace GrandTheftSpace.CoreGame.UserInterface
{
    internal class MenuManager : ScriptUtility, IDisposable
    {
        public MenuManager(Script script) : base(script)
        {
            script.Tick += OnTick;
            script.KeyUp += OnKeyUp;

            Init();
        }

        /// <summary>
        /// The main GTS menu.
        /// </summary>
        public NativeMenu MainMenu { get; private set; }

        /// <summary>
        /// The menu manager.
        /// </summary>
        public NativeMenuManager NativeMenuManager { get; private set; }

        /// <summary>
        /// The level editor menu sub-class.
        /// </summary>
        public LevelEditorMenu LevelEditorMenu { get; private set; }

        /// <summary>
        /// The key that opens the main menu.
        /// </summary>
        public Keys MenuKey { get; private set; }

        private void Init()
        {
            NativeMenuManager = new NativeMenuManager();
            MainMenu = new NativeMenu("Grand Theft Space", $"VERSION {CoreScript.VersionNum}", MenuBannerType.GunRunningGunMod);
            MainMenu.Init();
            NativeMenuManager.AddMenu(MainMenu);

            LevelEditorMenu = new LevelEditorMenu(this);
            LevelEditorMenu.AddToMenu(MainMenu, NativeMenuManager);
        }

        public override void ReadSettings(ScriptSettings scriptSettings)
        {
            if (scriptSettings == null)
            {
                return;
            }

            MenuKey = Keys.F11;
            MenuKey = scriptSettings.GetValue("menu", "main_menu_key", MenuKey);
            scriptSettings.SetValue("menu", "main_menu_key", MenuKey);
        }

        #region Script Events

        private void OnTick(object sender, EventArgs e)
        {
            NativeMenuManager.ProcessMenus();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != MenuKey)
            {
                return;
            }

            if (NativeMenuManager.IsAnyMenuOpen())
            {
                return;
            }

            MainMenu.Visible = !MainMenu.Visible;
        }

        #endregion

        #region IDisposable

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    MainMenu.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
