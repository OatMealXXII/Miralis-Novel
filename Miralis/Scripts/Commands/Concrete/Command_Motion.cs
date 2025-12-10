using VSNL.Commands;
using Cysharp.Threading.Tasks;
using VSNL.Characters;
using VSNL.Core;

namespace VSNL.Commands.Concrete
{
    public class Command_Motion : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // Syntax: "CharID" "MotionName"
            string[] parts = args.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return;

            string charId = parts[0].Trim('"');
            string motionName = parts[1].Trim('"');

            var mgr = Engine.Instance.GetService<CharacterManager>();
            if (mgr != null)
            {
                var view = mgr.GetCharacterView(charId); 
                if (view != null)
                {
                    view.PlayMotion(motionName);
                }
            }
            await UniTask.CompletedTask;
        }
    }
}

