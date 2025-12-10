using UnityEngine;
using Cysharp.Threading.Tasks;

namespace VSNL.Core
{
    public interface IResourceProvider
    {
        UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object;
        UniTask PreloadAsync(string pathOrLabel);
        void Unload(string path);
    }
}
