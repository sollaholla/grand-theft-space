using System.Collections.Generic;

namespace GrandTheftSpace.CoreGame.Serialization.Space
{
    public class SpaceLevel : Level
    {
        public float Gravity { get; set; }
        public List<Planet> Planets { get; set; }
    }
}
