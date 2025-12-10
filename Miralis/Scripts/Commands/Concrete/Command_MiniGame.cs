using VSNL.Commands;
using Cysharp.Threading.Tasks;
using VSNL.Services;
using VSNL.Core;

namespace VSNL.Commands.Concrete
{
    public class Command_MiniGame : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
             // Syntax: "SceneName" win_var:$var
            string[] parts = args.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            string sceneName = parts.Length > 0 ? parts[0].Trim('"') : "";
            string resultVar = null;

            foreach (var p in parts)
            {
                if (p.StartsWith("win_var:"))
                {
                     resultVar = p.Substring(8).Replace("$", "");
                }
            }

            var mgr = Engine.Instance.GetService<MiniGameManager>();
            if (mgr != null && !string.IsNullOrEmpty(sceneName))
            {
                await mgr.PlayMiniGameAsync(sceneName, resultVar);
            }
        }
    }
}

