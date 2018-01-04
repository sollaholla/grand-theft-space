using GrandTheftSpace.CoreGame.Serialization.Interfaces;
using GTA.Math;
using System.Collections.Generic;

namespace GrandTheftSpace.CoreGame.Serialization
{
    public class Level : ILevelTime, ILevelWeather, IPositionable
    {
        public Vector3 Position { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public bool FreezeTime { get; set; }
        public string Weather { get; set; }
        public List<TimecycleArea> TimecycleAreas { get; set; }
        public List<LevelTeleport> LevelTeleports { get; set; }
        public List<Teleport> Teleports { get; set; }
    }
}
