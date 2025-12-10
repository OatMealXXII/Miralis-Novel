using VSNL.Commands;
using Cysharp.Threading.Tasks;
using VSNL.Core;

namespace VSNL.Commands.Concrete
{
    public class Command_Preload : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            string path = args.Trim('"');
            if (string.IsNullOrEmpty(path)) return;

            var mgr = Engine.Instance.GetService<ResourceManager>();
            if (mgr != null)
            {
                await mgr.PreloadAsync(path);
            }
        }
    }
}

