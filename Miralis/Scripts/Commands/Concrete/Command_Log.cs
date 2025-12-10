using VSNL.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VSNL.Commands.Concrete
{
    public class Command_Log : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            Debug.Log($"[VSNL LOG] {args}");
            await UniTask.CompletedTask;
        }
    }
}

