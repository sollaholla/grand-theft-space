using GrandTheftSpace.CoreGame.Interfaces;
using GTA;

namespace GrandTheftSpace.CoreGame
{
    public abstract class ScriptUtility : IScriptUtility
    {
        protected ScriptUtility(Script script)
        {
            Script = script;
        }

        /// <summary>
        /// The script that instantiated this class instance.
        /// </summary>
        public Script Script { get; set; }

        /// <summary>
        /// Read the script settings file.
        /// </summary>
        public abstract void ReadSettings(ScriptSettings scriptSettings);
    }
}
