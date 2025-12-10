using VSNL.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VSNL.Core;
using VSNL.UI;
using VSNLEngine.Core;

namespace VSNL.Commands.Concrete
{
    public class Command_Stop : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            var ui = Engine.Instance.GetService<UIManager>();
            var player = Engine.Instance.GetService<ScriptPlayer>();

            if (ui != null)
            {
                ui.ShowChoices();
            }
            
            if (player != null)
            {
                player.PauseForChoice();
            }

            await UniTask.CompletedTask;
        }
    }
}

