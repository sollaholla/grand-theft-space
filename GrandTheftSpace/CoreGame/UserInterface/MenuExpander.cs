using GrandTheftSpace.CoreGame.UserInterface.Interfaces;
using GTAMenu;

namespace GrandTheftSpace.CoreGame.UserInterface
{
    internal abstract class MenuExpander : IMenuExpander
    {
        protected MenuExpander(MenuManager menuManager)
        {
            MenuManager = menuManager;
        }

        /// <summary>
        /// The menu manager who owns this subclass.
        /// </summary>
        public MenuManager MenuManager { get; set; }

        /// <summary>
        /// The main menu.
        /// </summary>
        public NativeMenu MainMenu { get; set; }

        /// <summary>
        /// Use this when you want to initialize your menu items and add items/sub-menus.
        /// </summary>
        /// <param name="mainMenu"></param>
        /// <param name="menuManager"></param>
        public abstract void AddToMenu(NativeMenu mainMenu, NativeMenuManager menuManager);
    }
}
