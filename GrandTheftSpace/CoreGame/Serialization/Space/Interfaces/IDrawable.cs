using GrandTheftSpace.CoreGame.Serialization.Interfaces;

namespace GrandTheftSpace.CoreGame.Serialization.Space.Interfaces
{
    public interface IDrawable : IPlacable
    {
        string Model { get; set; }
    }
}
