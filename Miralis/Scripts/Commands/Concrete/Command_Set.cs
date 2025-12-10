using VSNL.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VSNL.Core;

namespace VSNL.Commands.Concrete
{
    public class Command_Set : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // Syntax: $var = value
            var parts = args.Split('=');
            if (parts.Length < 2)
            {
                Debug.LogError($"[Command_Set] Invalid syntax: {args}");
                return;
            }

            string varName = parts[0].Trim();
            string expr = parts[1].Trim();

            // Resolve all variables in expression first
            var vm = Engine.Instance.GetService<VariableManager>();
            
            // Regex to find $vars and replace them
            // Note: VariableManager.ParseText does exactly this
            string resolvedExpr = vm.ParseText(expr);

            // Now evaluate
            object result = VSNL.Core.ExpressionEvaluator.Evaluate(resolvedExpr);
            
            // Format for storage (clean string)
            string storageValue = result.ToString();
            // If float, maybe keep formatting? 
            if (result is float f) storageValue = f.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (result is bool b) storageValue = b.ToString().ToLower();

            vm.SetVariable(varName, storageValue);
            Debug.Log($"[VSNL] Set {varName} to {storageValue} (Expr: {resolvedExpr})");

            await UniTask.CompletedTask;
        }
    }
}

