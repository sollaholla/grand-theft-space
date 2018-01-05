using GrandTheftSpace.CoreGame.Serialization.Space;
using GTA;
using GTA.Native;

namespace GrandTheftSpace.CoreGame.Gameplay.EntityTypes
{
    internal class PlanetEntity : PropEntity
    {
        private float rotationModifier;

        public PlanetEntity(Prop prop, Planet planetMetadata) : base(prop)
        {
            PlanetMetadata = planetMetadata;
        }

        public Planet PlanetMetadata { get; private set; }

        public override void Update()
        {
            SetPosition();
            SetRotation();
        }

        private void SetPosition()
        {
            var position = InitialPosition + PlanetMetadata.Offset;
            Function.Call(Hash.SET_ENTITY_COORDS, Handle, position.X, position.Y, position.Z);
        }

        private void SetRotation()
        {
            Rotation = PlanetMetadata.Rotation + (PlanetMetadata.AngularVelocity * rotationModifier);
            rotationModifier += Game.LastFrameTime % float.MaxValue;
        }
    }
}
