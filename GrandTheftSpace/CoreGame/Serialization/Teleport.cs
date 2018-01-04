using GrandTheftSpace.CoreGame.Serialization.Interfaces;
using GTA.Math;

namespace GrandTheftSpace.CoreGame.Serialization
{
    public class Teleport : IInteractable
    {
        public float TriggerDistance { get; set; }
        public Vector3 Offset { get; set; }
        public Vector3 NextOffset { get; set; }
        public bool TwoWay { get; set; }
    }
}
