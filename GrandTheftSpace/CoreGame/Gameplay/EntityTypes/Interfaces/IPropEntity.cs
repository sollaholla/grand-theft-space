using GTA;

namespace GrandTheftSpace.CoreGame.Gameplay.EntityTypes.Interfaces
{
    internal interface IPropEntity
    {
        Prop Prop { get; set; }

        void Update();
    }
}
