using GrandTheftSpace.CoreGame.Gameplay.EntityTypes;
using GTA;
using GTA.Math;
using System.Collections.Generic;

namespace GrandTheftSpace.CoreGame.Gameplay.RuntimeLevelUtilities
{
    internal class LevelPropManager : RuntimeLevelUtility
    {
        public LevelPropManager(RuntimeLevel runtimeLevel) : base(runtimeLevel)
        {
            Planets = new List<PlanetEntity>();
        }

        /// <summary>
        /// The planets instantiated by this level.
        /// </summary>
        public List<PlanetEntity> Planets { get; private set; }

        #region IUpdatable

        public override void Init()
        {
            CreateProps();
        }

        public override void Tick()
        {
            UpdateProps();
        }

        public override void Stop()
        {
            DeleteProps();
        }

        public override void Abort()
        { }

        #endregion

        private void UpdateProps()
        {
            foreach (var planet in Planets)
            {
                planet.Update();
            }
        }

        private void CreateProps()
        {
            CreatePlanets();
        }

        private void CreatePlanets()
        {
            if (LevelMetadata.Planets == null)
            {
                return;
            }

            var position = LevelMetadata.Position;

            foreach (var planetMeta in LevelMetadata.Planets)
            {
                var modelName = planetMeta.Model;

                var spawnPos = position + planetMeta.Offset;

                var prop = CreateProp(modelName, spawnPos);

                if (prop == null)
                {
                    continue;
                }

                prop.Rotation = planetMeta.Rotation;

                var planetEntity = new PlanetEntity(prop, planetMeta);

                Planets.Add(planetEntity);
            }
        }

        private Prop CreateProp(string modelName, Vector3 spawnPos)
        {
            var model = new Model(modelName);

            if (!model.IsValid)
            {
                return null;
            }

            model.Request(10000);

            return World.CreateProp(modelName, spawnPos, false, false);
        }

        private void DeleteProps()
        {
            foreach (var planet in Planets)
            {
                planet.Delete();
            }
        }
    }
}
