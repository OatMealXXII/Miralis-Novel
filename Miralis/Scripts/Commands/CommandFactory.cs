using System;
using System.Collections.Generic;
using UnityEngine;
using VSNL.Commands.Concrete;
using VSNL.Services;

namespace VSNL.Commands
{
    public static class CommandFactory
    {
        private static readonly Dictionary<string, Type> _commandRegistry = new Dictionary<string, Type>()
        {
            { "wait", typeof(Command_Wait) },
            { "log", typeof(Command_Log) },
            { "goto", typeof(Command_Goto) },
            { "set", typeof(Command_Set) },
            { "let", typeof(Command_Set) }, // Alias for convenience, though VSNL spec distinguishes mutability (ignored for now)
            { "if", typeof(Command_If) },
            { "char", typeof(Command_Char) },
            { "choice", typeof(Command_Choice) },
            { "stop", typeof(Command_Stop) },
            { "bg", typeof(Command_Bg) },
            { "bgm", typeof(Command_Bgm) },

            { "sfx", typeof(Command_Sfx) },
            { "event", typeof(Command_Event) },
            { "preload", typeof(Command_Preload) },
            { "movie", typeof(Command_Movie) },
            { "spawn", typeof(Command_Spawn) },
            { "despawn", typeof(Command_Despawn) },
            { "trans", typeof(Command_Trans) },
            { "motion", typeof(Command_Motion) },
            { "minigame", typeof(Command_MiniGame) }
        };

        public static IVSNLCommand Create(string commandName)
        {
            if (_commandRegistry.TryGetValue(commandName.ToLower(), out Type type))
            {
                return (IVSNLCommand)Activator.CreateInstance(type);
            }

            Debug.LogWarning($"[CommandFactory] Unknown command: {commandName}");
            return null;
        }

        public static IEnumerable<string> GetCommandNames()
        {
            return _commandRegistry.Keys;
        }
    }
}
