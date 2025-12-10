using VSNL.Commands;
using Cysharp.Threading.Tasks;
using VSNL.Core;
using VSNL.Services;

namespace VSNL.Commands.Concrete
{
    public class Command_Audio : IVSNLCommand
    {
        private bool _isSfx;

        public Command_Audio(bool isSfx)
        {
            _isSfx = isSfx;
        }

        public async UniTask ExecuteAsync(string args)
        {
            var audioManager = Engine.Instance.GetService<AudioManager>();
            if (audioManager != null)
            {
                if (_isSfx) audioManager.PlaySFX(args.Trim());
                else audioManager.PlayBGM(args.Trim());
            }
            await UniTask.CompletedTask;
        }
    }
}

