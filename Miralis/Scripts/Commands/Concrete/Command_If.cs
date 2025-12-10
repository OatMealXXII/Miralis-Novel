using VSNL.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VSNL.Core;
using VSNL.State; // For VariableManager
using VSNLEngine.Core; // For ScriptPlayer

namespace VSNL.Commands.Concrete
{
    public class Command_If : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // Syntax: expression (e.g. $score > 10 && $flag == true)
            var vm = Engine.Instance.GetService<VariableManager>();
            
            // 1. Resolve variables
            string resolvedExpr = vm.ParseText(args);
            
            // 2. Evaluate
            // 2. Evaluate
            object result = VSNL.Core.ExpressionEvaluator.Evaluate(resolvedExpr);
            
            bool conditionMet = false;
            if (result is bool b) conditionMet = b;
            else if (result is float f) conditionMet = f != 0f; // C-style
            else if (result is string s) conditionMet = !string.IsNullOrEmpty(s) && s.ToLower() != "false";

            if (!conditionMet)
            {
                // Skip to @else or @endif
                var player = Engine.Instance.GetService<ScriptPlayer>();
                player.SkipToNextCommand(new[] { "else", "endif" });
            }

            await UniTask.CompletedTask;
        }
    }
}

