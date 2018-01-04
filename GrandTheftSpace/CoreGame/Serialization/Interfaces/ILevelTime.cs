namespace GrandTheftSpace.CoreGame.Serialization.Interfaces
{
    public interface ILevelTime
    {
        int Hour { get; set; }
        int Minute { get; set; }
        bool FreezeTime { get; set; }
    }
}
