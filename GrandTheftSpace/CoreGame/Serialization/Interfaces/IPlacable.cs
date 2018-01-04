using GTA.Math;

namespace GrandTheftSpace.CoreGame.Serialization.Interfaces
{
    public interface IPlacable : IOffsetable
    {
        Vector3 Rotation { get; set; }
    }
}
