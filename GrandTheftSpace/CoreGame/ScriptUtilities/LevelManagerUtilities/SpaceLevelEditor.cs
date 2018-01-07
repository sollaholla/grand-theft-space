using GrandTheftSpace.CoreGame.Serialization.Space;
using GrandTheftSpace.CoreGame.Gameplay.Interfaces;
using GrandTheftSpace.CoreGame.UserInterface;
using GTAMenu;
using GTA;
using GrandTheftSpace.CoreGame.Interfaces;
using System.IO;
using GrandTheftSpace.CoreGame.Storage;
using System.Collections.Generic;
using System;
using GrandTheftSpace.CoreGame.Library;
using GTA.Math;
using GrandTheftSpace.CoreGame.Gameplay;
using GrandTheftSpace.CoreGame.Gameplay.EntityTypes;
using System.Reflection;
using System.Globalization;
using GrandTheftSpace.CoreGame.Serialization;

namespace GrandTheftSpace.CoreGame.ScriptUtilities.LevelManagerUtilities
{
    internal class SpaceLevelEditor : ISettingsReader, IUpdatable
    {
        private RuntimeLevel activateLevel;                       // The currently active level.
        private SpaceLevel activateLevelMetadata;                 // The currently active level's metadata.
        private Dictionary<Entity, NativeMenu> entityEditMenus;   // The menus that are bound to entities.

        public SpaceLevelEditor(MenuManager menuManager)
        {
            entityEditMenus = new Dictionary<Entity, NativeMenu>();

            MainMenu = new NativeMenu("Level Editor", "MAIN MENU", menuManager.MainMenu.BannerType)
            {
                AcceleratedScrolling = true,
                MaxDrawableItems = 10,
            };
            MainMenu.Init();

            menuManager.NativeMenuManager.AddMenu(MainMenu);

            MenuManager = menuManager;

            CameraManager = new SpaceLevelEditorCameraManager();
        }

        /// <summary>
        /// True if the current active level metadata exists.
        /// </summary>
        public bool Activate {
            get {
                return activateLevelMetadata != null;
            }
        }

        /// <summary>
        /// The menu manager.
        /// </summary>
        public MenuManager MenuManager { get; private set; }

        /// <summary>
        /// The main menu of the space level editor.
        /// </summary>
        public NativeMenu MainMenu { get; private set; }

        /// <summary>
        /// A menu that has all the entities spawned in the world.
        /// </summary>
        public NativeMenu EntityDatabaseMenu { get; private set; }

        /// <summary>
        /// Manages the space level camera.
        /// </summary>
        public SpaceLevelEditorCameraManager CameraManager { get; private set; }

        #region IUpdatable

        /// <summary>
        /// Closes all menus and opens the level editor menu.
        /// </summary>
        public void Init()
        {
            MenuManager.NativeMenuManager.CloseAllMenus();

            MainMenu.Visible = true;

            CameraManager.Init();
        }

        public void Tick()
        {
            CameraManager.Tick();

            if (!MenuManager.NativeMenuManager.IsAnyMenuOpen())
            {
                if (Game.IsControlJustPressed(2, Control.InteractionMenu))
                {
                    MainMenu.Visible = true;
                }
            }
        }

        public void Stop()
        {
            StopEditing();
        }

        public void Abort()
        {
            CameraManager.Abort();
        }

        #endregion

        #region ISettingsReader

        public void ReadSettings(ScriptSettings scriptSettings)
        {
            CameraManager.ReadSettings(scriptSettings);
        }

        #endregion

        #region Menu Events

        private void OnAddPlanetsMenuOpened(object sender, EventArgs e)
        {
            AddPlanetItems((NativeMenu)sender);
        }

        private void OnEntityDatabaseOpened(object sender, EventArgs e)
        {
            InitEntityDatabase();
        }

        private void OnEntitySpawnItemSelected(object sender, NativeMenuItemEventArgs e)
        {
            if (e.MenuItem.Tag is string model)
            {
                var prop = GTAUtil.CreateProp(model, activateLevelMetadata.Position);

                if (prop != null)
                {
                    var spawnOffset = CameraManager.LevelEditorCamera.Direction * 1500;

                    var spawnPos = CameraManager.LevelEditorCamera.Position + spawnOffset;

                    var planetMeta = new Planet
                    {
                        Model = model,
                        Offset = spawnPos - activateLevelMetadata.Position,
                    };

                    if (activateLevelMetadata.Planets == null)
                    {
                        activateLevelMetadata.Planets = new List<Planet>();
                    }

                    activateLevelMetadata.Planets.Add(planetMeta);

                    if (activateLevel.PropManager.Planets == null)
                    {
                        activateLevel.PropManager.Planets = new List<PlanetEntity>();
                    }

                    var planetEntity = new PlanetEntity(prop, planetMeta);

                    activateLevel.PropManager.Planets.Add(planetEntity);

                    var menu = CreateMenuForObject(planetMeta, model);

                    menu.Init();

                    MenuManager.NativeMenuManager.AddMenu(menu);

                    MenuManager.NativeMenuManager.CloseAllMenus();

                    menu.Visible = true;

                    AddEditMenuBinding(planetEntity, menu);

                    InitEntityDatabase();
                }
            }
        }

        private void OnEditMenuItemNavigated(object sender, NativeMenuItemUnmanagedNavigateIndexEventArgs e)
        {
            if (e.MenuItem.Tag is Tuple<PropertyInfo, object> propertyInfo)
            {
                var newValue = GetNextValueForType(propertyInfo.Item1.PropertyType, propertyInfo.Item1.GetValue(propertyInfo.Item2), e.LeftRight);

                if (newValue != null)
                {
                    propertyInfo.Item1.SetValue(propertyInfo.Item2, newValue);

                    e.MenuItem.Value = newValue;
                }
            }
            else if (e.MenuItem.Tag is Tuple<FieldInfo, object> fieldInfo)
            {
                var newValue = GetNextValueForType(fieldInfo.Item1.FieldType, fieldInfo.Item1.GetValue(fieldInfo.Item2), e.LeftRight);

                if (newValue != null)
                {
                    fieldInfo.Item1.SetValue(fieldInfo.Item2, newValue);

                    e.MenuItem.Value = newValue;
                }
            }
        }

        private void OnEditMenuItemSelected(object sender, NativeMenuItemEventArgs e)
        {
            var menuItem = e.MenuItem;

            if (menuItem.Tag is Tuple<PropertyInfo, object> propertyInfo)
            {
                var result = GetInputResult(propertyInfo.Item1.PropertyType, propertyInfo.Item1.GetValue(propertyInfo.Item2));

                if (result != null)
                {
                    propertyInfo.Item1.SetValue(propertyInfo.Item2, result);

                    menuItem.Value = result;
                }
            }
            else if (menuItem.Tag is Tuple<FieldInfo, object> fieldInfo)
            {
                var result = GetInputResult(fieldInfo.Item1.FieldType, fieldInfo.Item1.GetValue(fieldInfo.Item2));

                if (result != null)
                {
                    fieldInfo.Item1.SetValue(fieldInfo.Item2, result);

                    menuItem.Value = result;
                }
            }
        }

        private void OnSaveSelected(object sender, NativeMenuItemEventArgs e)
        {
            Serializer.Serialize(activateLevelMetadata, activateLevelMetadata.FilePath);

            UI.Notify("Saved!");
        }

        #endregion

        /// <summary>
        /// Initializes the edit mode menu, and sets the activate level metadata.
        /// </summary>
        /// <param name="levelMetadata"></param>
        public void BeginEdit(RuntimeLevel level)
        {
            if (Activate)
            {
                return;
            }

            activateLevel = level;
            activateLevelMetadata = level.LevelMetadata;

            CreateMenu();
        }

        private void StopEditing()
        {
            if (!Activate)
            {
                return;
            }

            CameraManager.Stop();

            activateLevel = null;
            activateLevelMetadata = null;
        }

        private void CreateMenu()
        {
            MainMenu.MenuItems.Clear();

            var addPlanetMenu = MenuManager.NativeMenuManager.AddSubMenu("Level Editor", "SELECT A MODEL", "Add Planet", "Add a planet that's defined in Planets.txt", MainMenu);

            addPlanetMenu.Init();

            addPlanetMenu.MenuOpened += OnAddPlanetsMenuOpened;

            EntityDatabaseMenu = MenuManager.NativeMenuManager.AddSubMenu("Level Editor", "SELECT AN OBJECT", "Entity Database", "Select and edit the currently active entities.", MainMenu);

            EntityDatabaseMenu.Init();

            EntityDatabaseMenu.MenuOpened += OnEntityDatabaseOpened;

            var saveItem = new NativeMenuItemBase("Save", "Save the current level.", null, ShopIcon.ShopHealthIcon);

            MainMenu.MenuItems.Add(saveItem);

            saveItem.Selected += OnSaveSelected;
        }

        private void InitEntityDatabase()
        {
            EntityDatabaseMenu.MenuItems.Clear();

            if (activateLevel.PropManager.Planets != null)
            {
                foreach (var planet in activateLevel.PropManager.Planets)
                {
                    var planetMenu = CreateMenuForObject(planet.PlanetMetadata, planet.PlanetMetadata.Model);

                    planetMenu.Init();

                    MenuManager.NativeMenuManager.AddSubMenu(planetMenu, planet.PlanetMetadata.Model, string.Empty, EntityDatabaseMenu);

                    AddEditMenuBinding(planet, planetMenu);
                }
            }
        }

        private NativeMenu CreateMenuForObject(object obj, string modelName)
        {
            var type = obj.GetType();

            var properties = type.GetProperties();

            var menu = new NativeMenu(modelName, "EDIT MODE", MainMenu.BannerType);

            foreach (var property in properties)
            {
                var pType = property.PropertyType;

                NativeMenuItemBase item = null;

                if (pType == typeof(int))
                {
                    item = new NativeMenuUnmanagedListItem(property.Name, string.Empty, property.GetValue(obj));
                }
                else if (pType == typeof(float))
                {
                    item = new NativeMenuUnmanagedListItem(property.Name, string.Empty, property.GetValue(obj));
                }
                else if (pType.IsEnum)
                {
                    item = new NativeMenuUnmanagedListItem(property.Name, string.Empty, property.GetValue(obj));
                }
                else if (pType == typeof(bool))
                {
                    item = new NativeMenuCheckboxItem(property.Name, string.Empty, (bool)property.GetValue(obj));
                }
                else if (pType == typeof(Vector3))
                {
                    var subMenu = MenuManager.NativeMenuManager.AddSubMenu(property.Name, "EDIT VECTOR", property.Name + " ~g~>>", string.Empty, menu);
                    var vector = (Vector3)property.GetValue(obj);

                    var xItem = new NativeMenuUnmanagedListItem("X", string.Empty, vector.X);
                    var yItem = new NativeMenuUnmanagedListItem("Y", string.Empty, vector.Y);
                    var zItem = new NativeMenuUnmanagedListItem("Z", string.Empty, vector.Z);
                    var persicionItem = new NativeMenuListItem("Precision", string.Empty, new object[] { 1000, 100, 10, 1, 0.1f, 0.01f, 0.001f, 0.0001f }, 3);

                    #region Index Changed Event

                    xItem.ChangedIndex += (sender, e) =>
                    {
                        if (float.TryParse(persicionItem.CurrentValue, NumberStyles.Any, CultureInfo.InstalledUICulture, out var f))
                        {
                            vector.X += (float)Math.Round(e.LeftRight * f, 4);
                            xItem.Value = vector.X;
                            property.SetValue(obj, vector);
                        }
                    };
                    yItem.ChangedIndex += (sender, e) =>
                    {
                        if (float.TryParse(persicionItem.CurrentValue, NumberStyles.Any, CultureInfo.InstalledUICulture, out var f))
                        {
                            vector.Y += (float)Math.Round(e.LeftRight * f, 4);
                            yItem.Value = vector.Y;
                            property.SetValue(obj, vector);
                        }
                    };
                    zItem.ChangedIndex += (sender, e) =>
                    {
                        if (float.TryParse(persicionItem.CurrentValue, NumberStyles.Any, CultureInfo.InstalledUICulture, out var f))
                        {
                            vector.Z += (float)Math.Round(e.LeftRight * f, 4);
                            zItem.Value = vector.Z;
                            property.SetValue(obj, vector);
                        }
                    };

                    #endregion

                    #region Selected Event

                    xItem.Selected += (sender, e) =>
                    {
                        var input = Game.GetUserInput(vector.X.ToString(), 99);

                        if (float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
                        {
                            vector.X = f;
                            xItem.Value = vector.X;
                            property.SetValue(obj, vector);
                        }
                    };
                    yItem.Selected += (sender, e) =>
                    {
                        var input = Game.GetUserInput(vector.Y.ToString(), 99);

                        if (float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
                        {
                            vector.Y = f;
                            yItem.Value = vector.Y;
                            property.SetValue(obj, vector);
                        }
                    };
                    zItem.Selected += (sender, e) =>
                    {
                        var input = Game.GetUserInput(vector.Z.ToString(), 99);

                        if (float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
                        {
                            vector.Z = f;
                            zItem.Value = vector.Z;
                            property.SetValue(obj, vector);
                        }
                    };

                    #endregion

                    subMenu.MenuItems.Add(xItem);
                    subMenu.MenuItems.Add(yItem);
                    subMenu.MenuItems.Add(zItem);
                    subMenu.MenuItems.Add(persicionItem);

                    subMenu.Init();
                }
                else if (pType == typeof(string))
                {
                    item = new NativeMenuItemBase(property.Name, string.Empty, property.GetValue(obj));
                }

                if (item != null)
                {
                    item.Tag = new Tuple<PropertyInfo, object>(property, obj);

                    if (item is NativeMenuUnmanagedListItem listItem)
                    {
                        listItem.ChangedIndex += OnEditMenuItemNavigated;
                    }

                    item.Selected += OnEditMenuItemSelected;

                    menu.MenuItems.Add(item);
                }
            }

            return menu;
        }

        private void AddPlanetItems(NativeMenu menu)
        {
            const string textFile = "Planets.txt";

            if (!Directory.Exists(Paths.Models))
            {
                return;
            }

            if (!File.Exists(Paths.Models + textFile))
            {
                return;
            }

            menu.MenuItems.Clear();

            var text = File.ReadAllLines(Paths.Models + textFile);

            foreach (var line in text)
            {
                var model = new Model(line);

                if (!model.IsValid)
                {
                    continue;
                }

                var menuItem = new NativeMenuItemBase(line, "Create " + line)
                {
                    Tag = line
                };

                menuItem.Selected += OnEntitySpawnItemSelected;

                menu.MenuItems.Add(menuItem);
            }
        }

        private void AddEditMenuBinding(Entity entity, NativeMenu menu)
        {
            if (entityEditMenus.ContainsKey(entity))
            {
                return;
            }

            entityEditMenus.Add(entity, menu);
        }

        private void RemoveEditMenuBinding(Entity entity)
        {
            if (!entityEditMenus.ContainsKey(entity))
            {
                return;
            }

            MenuManager.NativeMenuManager.RemoveMenu(entityEditMenus[entity]);

            entityEditMenus.Remove(entity);
        }

        private object GetNextValueForType(Type type, object currentValue, int leftRight)
        {
            object newValue = null;

            if (type == typeof(int))
            {
                newValue = ((int)currentValue + leftRight);
            }
            else if (type == typeof(float))
            {
                newValue = ((float)currentValue + (float)Math.Round(leftRight * 1f, 4));
            }
            else if (type.IsEnum)
            {
                var values = type.GetEnumValues();

                var index = Array.IndexOf(values, currentValue);

                index = (index + leftRight) % values.Length;

                if (index < 0)
                {
                    index = values.Length - 1;
                }

                newValue = values.GetValue(index);
            }

            return newValue;
        }

        private object GetInputResult(Type type, object currentValue)
        {
            object result = null;

            if (type == typeof(int))
            {
                var input = Game.GetUserInput(currentValue.ToString(), 99);

                input = input.Replace(',', '.');

                if (int.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var res))
                {
                    result = res;
                }
            }
            else if (type == typeof(float))
            {
                var input = Game.GetUserInput(currentValue.ToString(), 99);

                input = input.Replace(',', '.');

                if (float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var res))
                {
                    result = res;
                }
            }
            else if (type == typeof(string))
            {
                var input = Game.GetUserInput(currentValue.ToString(), 99);

                result = input;
            }

            return result;
        }
    }
}
