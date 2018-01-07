using GTA;

namespace GrandTheftSpace.CoreGame.Interfaces
{
    internal interface IScriptUtility : ISettingsReader
    {
        Script Script { get; set; }
    }
}
