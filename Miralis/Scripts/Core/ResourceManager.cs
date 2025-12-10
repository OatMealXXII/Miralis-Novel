using UnityEngine;
using Cysharp.Threading.Tasks;

namespace VSNL.Core
{
    public class ResourceManager : MonoBehaviour, IGameService
    {
        private IResourceProvider _provider;

        public async UniTask InitializeAsync()
        {
            // Default to ResourcesLoader
            _provider = new ResourcesLoader();
            Debug.Log("[ResourceManager] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            // No specific state to reset
        }

        public async UniTask<T> LoadAssetAsync<T>(string path) where T : UnityEngine.Object
        {
            if (_provider == null)
            {
                Debug.LogError("[ResourceManager] Provider not initialized.");
                return null;
            }
            try 
            {
                return await _provider.LoadAsync<T>(path);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ResourceManager] Failed to load {path}: {ex.Message}");
                return null;
            }
        }

        public async UniTask PreloadAsync(string pathOrLabel)
        {
            if (_provider != null) await _provider.PreloadAsync(pathOrLabel);
        }

        public void UnloadAsset(string path)
        {
            if (_provider != null) _provider.Unload(path);
        }

        public async UniTask UnloadUnusedAssets()
        {
             await Resources.UnloadUnusedAssets().ToUniTask();
        }
    }
}

