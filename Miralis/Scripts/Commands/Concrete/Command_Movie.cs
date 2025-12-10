using VSNL.Commands;
using Cysharp.Threading.Tasks;
using VSNL.Media;
using VSNL.Core;

namespace VSNL.Commands
{
    public class Command_Movie : IVSNLCommand
    {
        public async UniTask ExecuteAsync(string args)
        {
            // Syntax: "filename" "mode" (optional)
            // Example: "intro.mp4" "blocking"
            var parts = args.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            
            string fileName = "";
            string mode = "blocking";

            if (parts.Length > 0) fileName = parts[0].Trim('"');
            if (parts.Length > 1) mode = parts[1].Trim('"').ToLower();

            var mgr = Engine.Instance.GetService<VideoManager>();
            if (mgr != null)
            {
                bool isBlocking = mode == "blocking";
                bool isLooped = mode == "background";
                await mgr.PlayVideoAsync(fileName, isBlocking, isLooped);
            }
        }
    }
}

