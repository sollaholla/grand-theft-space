using GrandTheftSpace.CoreGame.Gameplay.EntityTypes.Interfaces;
using GTA;
using GTA.Math;

namespace GrandTheftSpace.CoreGame.Gameplay.EntityTypes
{
    internal abstract class PropEntity : Entity, IPropEntity
    {
        public PropEntity(Prop prop) : base(prop.Handle)
        {
            Prop = prop;
            InitialPosition = prop.Position;
        }

        public Vector3 InitialPosition { get; set; }

        public Prop Prop { get; set; }

        public abstract void Update();
    }
}
