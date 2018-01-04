using System;
using System.Windows.Forms;
using GTA;
using GrandTheftSpace.CoreGame.UserInterface;
using GrandTheftSpace.CoreGame.Debugging;

namespace GrandTheftSpace.CoreGame.ScriptThreads
{
    internal class CoreScript : Script
    {
        /// <summary>
        /// The mod version.
        /// </summary>
        public const string VersionNum = "2.0.0";

        public CoreScript()
        {
            Tick += OnTick;
            KeyUp += OnKeyUp;
            Aborted += OnAborted;

            Init();
        }

        /// <summary>
        /// Initializes the main menu and the native menu manager.
        /// </summary>
        public MenuManager MenuManager { get; private set; }

        private void Init()
        {
            MenuManager = new MenuManager(this);
            MenuManager.ReadSettings(Settings);
            InitSettings();

            Logger.Log("Initialized.");
        }

        private void InitSettings()
        {
            Settings.Save();
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
        }

        private void OnAborted(object sender, EventArgs eventArgs)
        {
            MenuManager.Dispose();
        }
    }
}
