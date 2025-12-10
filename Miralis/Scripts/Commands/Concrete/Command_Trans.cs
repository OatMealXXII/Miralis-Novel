using VSNL.Commands;
using Cysharp.Threading.Tasks;
using VSNL.Core;
using VSNL.UI;

namespace VSNL.Commands
{
    public class Command_Trans : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // Syntax: "Key" "DefaultText"
            var parts = args.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            
            string key = "";
            string defaultText = "";

            if (parts.Length > 0) key = parts[0].Trim('"');
            if (parts.Length > 1) defaultText = parts[1].Trim('"');

            var loc = Engine.Instance.GetService<LocalizationManager>();
            var ui = Engine.Instance.GetService<UIManager>(); 

            string text = loc != null ? loc.GetTranslation(key, defaultText) : defaultText;
            
            if (ui != null)
            {
               await ui.ShowDialogueAsync("", text); 
            }
        }
    }
}

