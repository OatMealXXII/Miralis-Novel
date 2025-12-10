using VSNL.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VSNL.Core;

namespace VSNL.Commands.Concrete
{
    public class Command_Event : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // Syntax: "ObjectName" "MethodName" "OptionalParameter"
             // Simple space splitter preserving quotes is hard, but for now simple split
            string[] parts = args.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            
            string objectName = parts.Length > 0 ? parts[0].Trim('"') : null;
            string methodName = parts.Length > 1 ? parts[1].Trim('"') : null;
            string parameter = parts.Length > 2 ? parts[2].Trim('"') : null;

            if (string.IsNullOrEmpty(objectName) || string.IsNullOrEmpty(methodName))
            {
                Debug.LogWarning("[Command_Event] Invalid syntax. Usage: @event \"Obj\" \"Method\" \"Param\"");
                return;
            }

            // Find object
            var obj = GameObject.Find(objectName);
            if (obj != null)
            {
                if (parameter != null)
                {
                    obj.SendMessage(methodName, parameter, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    obj.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
                }
                Debug.Log($"[Command_Event] Sent '{methodName}' to '{objectName}'");
            }
            else
            {
                Debug.LogWarning($"[Command_Event] GameObject '{objectName}' not found.");
            }

            await UniTask.CompletedTask;
        }
    }
}

