using GTA.Math;

namespace GrandTheftSpace.CoreGame.Serialization.Interfaces
{
    public interface ILevelArea
    {
        Vector3 Offset { get; set; }

        float Radius { get; set; }
    }
}
