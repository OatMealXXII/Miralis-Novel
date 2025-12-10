using VSNL.Commands;
using Cysharp.Threading.Tasks;
using VSNL.Services;
using VSNL.Core;
using VSNLEngine.Core; // If Engine is here

namespace VSNL.Commands.Concrete
{
    public class Command_Despawn : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            string key = args.Trim('"');
            var mgr = Engine.Instance.GetService<SpawnManager>();
            if (mgr != null)
            {
                mgr.Despawn(key);
            }
            await UniTask.CompletedTask;
        }
    }
}

