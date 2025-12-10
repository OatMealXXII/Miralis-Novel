using System.Collections.Generic;
using VSNL.Core;

namespace VSNLEngine.Core.Data
{
    /// <summary>
    /// Represents the parsed data of a VSNL script file.
    /// </summary>
    public class ScriptData
    {
        /// <summary>
        /// The name of the script.
        /// </summary>
        public string ScriptName { get; private set; }

        /// <summary>
        /// The list of parsed lines.
        /// </summary>
        public List<VSNLLine> Lines { get; private set; }

        /// <summary>
        /// A map of Label names to their line indices.
        /// </summary>
        public Dictionary<string, int> LabelMap { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptData"/> class.
        /// </summary>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="lines">parsed lines.</param>
        /// <param name="labelMap">Label mapping.</param>
        public ScriptData(string scriptName, List<VSNLLine> lines, Dictionary<string, int> labelMap)
        {
            ScriptName = scriptName;
            Lines = lines;
            LabelMap = labelMap;
        }
    }
}
