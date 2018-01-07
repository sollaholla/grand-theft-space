using System;
using GTAMenu;
using GrandTheftSpace.CoreGame.Storage;
using System.IO;
using GrandTheftSpace.CoreGame.Serialization;
using GrandTheftSpace.CoreGame.Serialization.Space;

namespace GrandTheftSpace.CoreGame.UserInterface.MenuExpansions.LevelSelectionMenuExpansions
{
    internal class OpenFileMenu : MenuExpander
    {
        public OpenFileMenu(MenuManager menuManager) : base(menuManager)
        { }

        public override void AddToMenu(NativeMenu mainMenu, NativeMenuManager menuManager)
        {
            MainMenu = menuManager.AddSubMenu(mainMenu.Title, "OPEN FILE", "Open", "Open a level xml file for editing.", mainMenu);
            MainMenu.Init();
            MainMenu.MenuOpened += OnMenuOpened;
        }

        private void OnMenuOpened(object sender, EventArgs e)
        {
            MainMenu.MenuItems.Clear();

            if (!Directory.Exists(Paths.Levels))
            {
                return;
            }

            var files = Directory.GetFiles(Paths.Levels);

            foreach (var fileName in files)
            {
                CreateMenuItemForFile(fileName);
            }
        }

        private void CreateMenuItemForFile(string fileName)
        {
            var level = Serializer.Deserialize<SpaceLevel>(fileName);

            if (level == null)
            {
                return;
            }

            level.FilePath = fileName;

            var menuItem = new NativeMenuItemBase(Path.GetFileNameWithoutExtension(fileName), "Select to create level from: " + fileName)
            {
                Tag = level
            };

            MainMenu.MenuItems.Add(menuItem);
        }
    }
}
