using VSNL.Commands;
using Cysharp.Threading.Tasks;
using VSNL.Core;
using VSNL.Services;

namespace VSNL.Commands.Concrete
{
    public class Command_Sfx : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            var audioManager = Engine.Instance.GetService<AudioManager>();
            if (audioManager != null)
            {
                audioManager.PlaySFX(args.Trim());
            }
            await UniTask.CompletedTask;
        }
    }
}

