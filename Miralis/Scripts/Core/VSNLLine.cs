using System.Text.RegularExpressions;
using UnityEngine;

namespace VSNL.Core
{
    public enum VSNLLineType
    {
        Comment,
        Command,
        Dialogue,
        Label,
        Unknown
    }

    /// <summary>
    /// Represents a single parsed line of a VSNL script.
    /// </summary>
    public class VSNLLine
    {
        public VSNLLineType Type { get; private set; }
        public string RawContent { get; private set; }
        
        // Command Data
        public string CommandName { get; private set; }
        public string CommandArgs { get; private set; }

        // Dialogue Data
        public string Speaker { get; private set; }
        public string DialogText { get; private set; }

        // Label Data
        public string LabelName { get; private set; }

        // Regex Patterns (Strict Reference Compliance)
        
        // 1. Label: [Name] (Reference)
        // Also supporting "label Name" for robustness if desired, but Reference says [Name]
        private const string REGEX_LABEL = @"^\[(\w+)\]$";

        // 2. Commands: Start with @ (e.g., @bg "Name")
        private const string REGEX_COMMAND = @"^@(\w+)(?:\s+(.*))?$"; 

        // 3. Dialogue: Speaker: "Text"
        // Reference: Elysia: "Text"
        private const string REGEX_DIALOGUE = @"^([^:]+):\s*""?(.*?)""?$";
        
        // 4. Implicit Dialogue: "Text" (Narrator/No Speaker)
        // Reference doesn't explicitly forbid, but implies Speaker: is standard.
        // We'll keep implicit support but mapped to null speaker.
        private const string REGEX_DIALOGUE_IMPLICIT = @"^""(.*)""$";

        public VSNLLine(string rawLine)
        {
            RawContent = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(RawContent) || RawContent.StartsWith("#") || RawContent.StartsWith("//"))
            {
                Type = VSNLLineType.Comment;
                return;
            }

            // 1. Label
            var labelMatch = Regex.Match(RawContent, REGEX_LABEL);
            if (labelMatch.Success)
            {
                Type = VSNLLineType.Label;
                LabelName = labelMatch.Groups[1].Value;
                return;
            }

            // 2. Command (Strict @)
            var cmdMatch = Regex.Match(RawContent, REGEX_COMMAND);
            if (cmdMatch.Success)
            {
                Type = VSNLLineType.Command;
                CommandName = cmdMatch.Groups[1].Value.ToLower();
                CommandArgs = cmdMatch.Groups[2].Value.Trim();
                return;
            }

            // 3. Dialogue (Speaker: "Text")
            var diagMatch = Regex.Match(RawContent, REGEX_DIALOGUE);
            if (diagMatch.Success)
            {
                Type = VSNLLineType.Dialogue;
                Speaker = diagMatch.Groups[1].Value.Trim();
                DialogText = diagMatch.Groups[2].Value.Trim();
                
                // Cleanup quotes
                if (DialogText.StartsWith("\"") && DialogText.EndsWith("\""))
                {
                    DialogText = DialogText.Substring(1, DialogText.Length - 2);
                }
                return;
            }
            
            // 4. Implicit Dialogue ("Text")
            var diagImpMatch = Regex.Match(RawContent, REGEX_DIALOGUE_IMPLICIT);
            if (diagImpMatch.Success)
            {
                Type = VSNLLineType.Dialogue;
                Speaker = null; 
                DialogText = diagImpMatch.Groups[1].Value;
                return;
            }

            Type = VSNLLineType.Unknown;
            Debug.LogWarning($"[VSNLLine] Could not parse line: {RawContent}");
        }
    }
}
