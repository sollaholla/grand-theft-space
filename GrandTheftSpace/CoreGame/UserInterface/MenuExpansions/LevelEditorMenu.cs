using GrandTheftSpace.CoreGame.UserInterface.MenuExpansions.LevelEditor;
using GTAMenu;

namespace GrandTheftSpace.CoreGame.UserInterface.MenuExpansions
{
    internal class LevelEditorMenu : MenuExpander
    {
        public LevelEditorMenu(MenuManager menuManager) : base(menuManager)
        { }

        /// <summary>
        /// Lists xml files within the levels folder.
        /// </summary>
        public OpenFileMenu OpenFileMenu { get; private set; }

        public override void AddToMenu(NativeMenu mainMenu, NativeMenuManager menuManager)
        {
            MainMenu = menuManager.AddSubMenu(mainMenu.Title, "LEVEL EDITOR", "Level Editor", "Create/Edit space levels.", mainMenu);
            MainMenu.Init();

            OpenFileMenu = new OpenFileMenu(MenuManager);
            OpenFileMenu.AddToMenu(MainMenu, menuManager);
        }
    }
}
