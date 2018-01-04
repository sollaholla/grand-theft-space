using GrandTheftSpace.CoreGame.ScriptThreads;
using GTA;
using System;
using System.IO;

namespace GrandTheftSpace.CoreGame.Debugging
{
    internal static class Logger
    {
        /// <summary>
        /// Path to the GTS log.
        /// </summary>
        public const string Path = @"scripts\GrandTheftSpace.log";

        /// <summary>
        /// The header of the GTS log.
        /// </summary>
        public const string Header = "Grand Theft Space " + CoreScript.VersionNum;

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            Log(ex.Message + Environment.NewLine + ex.StackTrace, LogType.ERROR);
        }

        /// <summary>
        /// Log some debug text.
        /// </summary>
        /// <param name="text"></param>
        public static void Log(string text)
        {
            Log(text, LogType.DEBUG);
        }

        /// <summary>
        /// The main log function.
        /// </summary>
        /// <param name="text">The text to log.</param>
        /// <param name="type">The log type.</param>
        public static void Log(string text, LogType type)
        {
            var initialText = $"{Header} {DateTime.Now} {Game.Version}";

            if (File.Exists(Path))
            {
                initialText = File.ReadAllText(Path);
            }

            initialText += $"{Environment.NewLine}[{type}] [{DateTime.Now}]: {text}";

            File.WriteAllText(Path, initialText);
        }
    }
}
