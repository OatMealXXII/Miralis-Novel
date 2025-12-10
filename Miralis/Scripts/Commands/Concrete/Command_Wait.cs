using VSNL.Commands;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VSNL.Commands.Concrete
{
    public class Command_Wait : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            if (float.TryParse(args, NumberStyles.Float, CultureInfo.InvariantCulture, out float duration))
            {
                // TODO: Check if Skip Mode is active to ignore wait using Engine.GetService...
                await UniTask.Delay(System.TimeSpan.FromSeconds(duration));
            }
            else
            {
                Debug.LogWarning($"[Command_Wait] Invalid duration: {args}");
            }
        }
    }
}

