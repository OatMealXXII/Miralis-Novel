using Cysharp.Threading.Tasks;
using VSNL.Commands;
using UnityEngine;
using VSNL.Core;
using VSNL.State;
using VSNL.Characters;

namespace VSNL.Commands.Concrete
{
    public class Command_Char : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // Syntax: Name:Emotion:Position (e.g. Elysia:Happy:Center)
            var parts = args.Split(':');
            if (parts.Length < 1)
            {
                Debug.LogError($"[Command_Char] Invalid args: {args}");
                return;
            }

            string name = parts[0];
            string emotion = parts.Length > 1 ? parts[1] : "Default";
            string position = parts.Length > 2 ? parts[2] : "Center";

            var charManager = Engine.Instance.GetService<CharacterManager>();
            if (charManager != null)
            {
                await charManager.ShowCharacterAsync(name, emotion, position);
            }
            else
            {
                Debug.LogError("[Command_Char] CharacterManager not found!");
            }

            await UniTask.CompletedTask;
        }
    }
}

