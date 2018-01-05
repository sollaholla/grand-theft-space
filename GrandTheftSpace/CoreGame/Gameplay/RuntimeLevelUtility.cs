using GrandTheftSpace.CoreGame.Gameplay.Interfaces;
using GrandTheftSpace.CoreGame.Serialization.Space;

namespace GrandTheftSpace.CoreGame.Gameplay
{
    internal abstract class RuntimeLevelUtility : IRuntimeLevelUtility
    {
        protected RuntimeLevelUtility(RuntimeLevel runtimeLevel)
        {
            RuntimeLevel = runtimeLevel;
            LevelMetadata = runtimeLevel.LevelMetadata;
        }

        /// <summary>
        /// The currentl level.
        /// </summary>
        public RuntimeLevel RuntimeLevel { get; private set; }

        /// <summary>
        /// The metadata of the current level.
        /// </summary>
        public SpaceLevel LevelMetadata { get; private set; }

        public abstract void Init();
        public abstract void Tick();
        public abstract void Stop();
        public abstract void Abort();
    }
}
