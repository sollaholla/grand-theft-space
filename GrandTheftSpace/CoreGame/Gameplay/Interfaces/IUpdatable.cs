namespace GrandTheftSpace.CoreGame.Gameplay.Interfaces
{
    internal interface IUpdatable
    {
        void Init();
        void Tick();
        void Stop();
        void Abort();
    }
}
