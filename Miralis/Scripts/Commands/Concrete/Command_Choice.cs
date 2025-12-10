using VSNL.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VSNL.Core;

namespace VSNL.Commands.Concrete
{
    public class Command_Choice : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // Syntax: "Option Text" -> LabelName
            // Example: "Go to Forest" -> ForestPath
            
            var parts = args.Split(new[] { "->" }, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                Debug.LogWarning($"[Command_Choice] Invalid syntax: {args}");
                return;
            }

            string text = parts[0].Trim().Replace("\"", ""); // remove quotes
            string label = parts[1].Trim();

            var ui = Engine.Instance.GetService<UIManager>();
            ui.AddChoice(text, label);

            await UniTask.CompletedTask;
        }
    }
}

