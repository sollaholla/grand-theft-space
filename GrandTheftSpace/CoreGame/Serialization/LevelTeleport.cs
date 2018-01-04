using GrandTheftSpace.CoreGame.Serialization.Interfaces;
using GTA.Math;

namespace GrandTheftSpace.CoreGame.Serialization
{
    public class LevelTeleport : IInteractable
    {
        public Vector3 Offset { get; set; }
        public float TriggerDistance { get; set; }
        public string NextLevel { get; set; }
    }
}
