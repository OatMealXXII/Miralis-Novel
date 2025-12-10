using System.Collections.Generic;
using UnityEngine;
using VSNL.Core; // Assuming VSNLLine is still here or needs moving. 
// Ideally VSNLLine should be in Data namespace too, but for now we reference it.
using VSNLEngine.Core.Data;

namespace VSNLEngine.Core.Parser
{
    /// <summary>
    /// Responsible for parsing raw text assets into executable ScriptData.
    /// </summary>
    public static class ScriptParser
    {
        /// <summary>
        /// Parses a Unity TextAsset into ScriptData.
        /// </summary>
        /// <param name="scriptFile">The script text asset.</param>
        /// <returns>A structured ScriptData object.</returns>
        public static ScriptData Parse(TextAsset scriptFile)
        {
            if (scriptFile == null)
            {
                Debug.LogError("[ScriptParser] Script file is null.");
                return new ScriptData("Empty", new List<VSNLLine>(), new Dictionary<string, int>());
            }

            var lines = scriptFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            var parsedLines = new List<VSNLLine>();
            var labelMap = new Dictionary<string, int>();

            for (int i = 0; i < lines.Length; i++)
            {
                var parsed = new VSNLLine(lines[i]);
                parsedLines.Add(parsed);

                if (parsed.Type == VSNLLineType.Label)
                {
                    if (!labelMap.ContainsKey(parsed.LabelName))
                    {
                        labelMap.Add(parsed.LabelName, i);
                    }
                    else
                    {
                        Debug.LogWarning($"[ScriptParser] Duplicate Label found: {parsed.LabelName}");
                    }
                }
            }

            return new ScriptData(scriptFile.name, parsedLines, labelMap);
        }
    }
}
