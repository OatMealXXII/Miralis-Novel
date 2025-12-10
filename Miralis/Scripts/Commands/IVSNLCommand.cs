using Cysharp.Threading.Tasks;

namespace VSNL.Commands
{
    public interface IVSNLCommand
    {
        UniTask ExecuteAsync(string args);
    }
}
