using VSNL.Commands;
using Cysharp.Threading.Tasks;
using VSNL.Core;
using VSNL.Services;

namespace VSNL.Commands.Concrete
{
    public class Command_Bg : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            var bgManager = Engine.Instance.GetService<BackgroundManager>();
            if (bgManager != null)
            {
                // Remove quotes if present
                string imageName = args.Trim().Replace("\"", "");
                await bgManager.SetBackgroundAsync(imageName);
            }
            await UniTask.CompletedTask;
        }
    }
}

