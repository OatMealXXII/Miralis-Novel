using UnityEngine;
using Cysharp.Threading.Tasks;

namespace VSNL.Core
{
    public class ResourcesLoader : IResourceProvider
    {
        public async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        {
            var request = Resources.LoadAsync<T>(path);
            await request.ToUniTask();
            return request.asset as T;
        }

        public async UniTask PreloadAsync(string pathOrLabel)
        {
            // Resources.Load doesn't really have a "preload" that persists without reference.
            // But we can load and reference it temporarily? 
            // For Resources, preloading isn't standard practice vs just loading.
            // We'll mimic it by loading and immediately letting go, hoping OS cache helps, 
            // or just no-op since Resources is synchronous-ish/fast enough usually.
            // A better "Preload" for Resources is storing it in a dictionary in this Loader.
            // But for this task, we'll keep it simple or no-op.
            await UniTask.CompletedTask;
        }

        public void Unload(string path)
        {
            // Resources API doesn't support specific unload easily unless it's an AssetBundle
            // or we use Resources.UnloadUnusedAssets().
            // We'll leave this empty for now, relying on UnloadUnusedAssets in Manager.
        }
    }
}
