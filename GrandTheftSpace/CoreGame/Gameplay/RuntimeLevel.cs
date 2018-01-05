using GrandTheftSpace.CoreGame.Gameplay.Interfaces;
using GrandTheftSpace.CoreGame.Serialization.Space;
using GrandTheftSpace.CoreGame.Gameplay.RuntimeLevelUtilities;

namespace GrandTheftSpace.CoreGame.Gameplay
{
    internal class RuntimeLevel : IUpdatable, ILevelMetadata
    {
        internal RuntimeLevel(SpaceLevel levelMetadata)
        {
            LevelMetadata = levelMetadata;
            GraphicsManager = new LevelGraphicsManager(this);
            PropManager = new LevelPropManager(this);
        }

        /// <summary>
        /// The level's metadata loaded from xml.
        /// </summary>
        public SpaceLevel LevelMetadata { get; private set; }

        /// <summary>
        /// Manages timecycle modifiers and graphical stuff.
        /// </summary>
        public LevelGraphicsManager GraphicsManager { get; private set; }

        /// <summary>
        /// Creates and updates the props created by the level metadata.
        /// </summary>
        public LevelPropManager PropManager { get; private set; }

        #region IUpdatable

        public void Init()
        {
            GraphicsManager.Init();
            PropManager.Init();
        }

        public void Tick()
        {
            GraphicsManager.Tick();
            PropManager.Tick();
        }

        public void Stop()
        {
            GraphicsManager.Stop();
            PropManager.Stop();
        }

        public void Abort()
        {
            GraphicsManager.Abort();
            PropManager.Abort();
        }

        #endregion
    }
}
