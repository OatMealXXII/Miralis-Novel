using VSNL.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VSNL.Core; // Keep just in case, though might be redundant
using VSNLEngine.Core; // For ScriptPlayer

namespace VSNL.Commands.Concrete
{
    public class Command_Goto : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            var label = args.Trim();
            var player = Engine.Instance.GetService<ScriptPlayer>();
            
            if (player != null)
            {
                player.JumpToLabel(label);
            }

            await UniTask.CompletedTask;
        }
    }
}

