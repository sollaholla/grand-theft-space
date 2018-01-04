using GrandTheftSpace.CoreGame.Serialization.Interfaces;
using GTA.Math;

namespace GrandTheftSpace.CoreGame.Serialization
{
    public class TimecycleArea : ILevelArea, ILevelTime, ILevelWeather
    {
        public Vector3 Offset { get; set; }

        public float Radius { get; set; }

        public string TimecycleModifier { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public bool FreezeTime { get; set; }

        public string Weather { get; set; }
    }
}
