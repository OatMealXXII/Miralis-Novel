using VSNL.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VSNL.Services;
using VSNL.Core;
using System.Globalization;

namespace VSNL.Commands
{
    public class Command_Spawn : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // Syntax: "Name" pos:x,y scale:x alias:"Name"
            var parts = args.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            
            string prefabName = "";
            Vector2 position = Vector2.zero;
            Vector3 scale = Vector3.one;
            string alias = null;

            if (parts.Length > 0) prefabName = parts[0].Trim('"');

            for (int i = 1; i < parts.Length; i++)
            {
                string param = parts[i].Trim();
                if (param.StartsWith("pos:"))
                {
                    var coords = param.Substring(4).Split(',');
                    if (coords.Length >= 2)
                    {
                        float.TryParse(coords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out float x);
                        float.TryParse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float y);
                        position = new Vector2(x, y);
                    }
                }
                else if (param.StartsWith("scale:"))
                {
                    string sVal = param.Substring(6);
                    if (float.TryParse(sVal, NumberStyles.Any, CultureInfo.InvariantCulture, out float s))
                    {
                        scale = Vector3.one * s;
                    }
                }
                else if (param.StartsWith("alias:"))
                {
                    alias = param.Substring(6).Trim('"');
                }
            }

            var mgr = Engine.Instance.GetService<SpawnManager>();
            if (mgr != null)
            {
                await mgr.SpawnAsync(prefabName, position, scale, alias);
            }
        }
    }
}

