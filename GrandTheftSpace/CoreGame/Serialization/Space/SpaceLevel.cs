using System.Collections.Generic;
using System.Xml.Serialization;

namespace GrandTheftSpace.CoreGame.Serialization.Space
{
    public class SpaceLevel : Level
    {
        public float Gravity { get; set; }

        public List<Planet> Planets { get; set; }

        [XmlIgnore]
        public string FilePath { get; set; }
    }
}
