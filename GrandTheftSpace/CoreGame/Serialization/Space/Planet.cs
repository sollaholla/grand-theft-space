using GrandTheftSpace.CoreGame.Serialization.Interfaces;
using GrandTheftSpace.CoreGame.Serialization.Space.Interfaces;
using GTA.Math;

namespace GrandTheftSpace.CoreGame.Serialization.Space
{
    public class Planet : IDrawable, ITriggerable, IRigidBody
    {
        public string Model { get; set; }
        public Vector3 Offset { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 AngularVelocity { get; set; }
        public float TriggerDistance { get; set; }
    }
}
