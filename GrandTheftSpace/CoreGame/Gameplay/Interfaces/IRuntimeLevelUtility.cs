using GrandTheftSpace.CoreGame.Serialization.Space;

namespace GrandTheftSpace.CoreGame.Gameplay.Interfaces
{
    internal interface IRuntimeLevelUtility : IUpdatable, ILevelMetadata
    {
        RuntimeLevel RuntimeLevel { get; }
    }
}
