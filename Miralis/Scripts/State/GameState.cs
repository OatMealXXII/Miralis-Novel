using System;
using System.Collections.Generic;
using UnityEngine;

namespace VSNL.State
{
    /// <summary>
    /// Serializable container for the entire game state.
    /// Used for Save/Load.
    /// </summary>
    [Serializable]
    public class GameState
    {
        // Script Execution State
        public string ScriptFileName;
        public int LineIndex;

        // Scene State
        public string CurrentBackground; // Resource Path
        public string CurrentMusic;

        // Character State
        public List<CharacterEntry> ActiveCharacters = new List<CharacterEntry>();

        // Variables
        public List<VariableEntry> GlobalVariables = new List<VariableEntry>();

        public GameState() { } // Empty constructor for serialization
    }

    [Serializable]
    public struct CharacterEntry
    {
        public string Name;
        public string Emotion;
        public string Position; // "Left", "Center", "Right" or "x,y"
        public bool IsVisible;
    }

    [Serializable]
    public struct VariableEntry
    {
        public string Key;
        public string Value;
    }
}
