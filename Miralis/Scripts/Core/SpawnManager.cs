using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using VSNL.Core;

namespace VSNL.Services
{
    public class SpawnManager : MonoBehaviour, IGameService
    {
        public Transform VFXRoot;
        private Dictionary<string, GameObject> _spawnedObjects = new Dictionary<string, GameObject>();

        public async UniTask InitializeAsync()
        {
            // Create Root
            if (!VFXRoot)
            {
                var canvas = FindFirstObjectByType<Canvas>();
                if (canvas)
                {
                    var rootObj = new GameObject("VFXRoot");
                    rootObj.transform.SetParent(canvas.transform, false);
                    
                    var rect = rootObj.AddComponent<RectTransform>();
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                    
                    // Raycast pass-through
                    // CanvasGroup not strictly needed unless we want alpha control
                    
                    VFXRoot = rootObj.transform;
                }
            }
            
            Debug.Log("[SpawnManager] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            DespawnAll();
        }

        public async UniTask SpawnAsync(string prefabName, Vector2 position, Vector3 scale, string alias = null)
        {
            string key = string.IsNullOrEmpty(alias) ? prefabName : alias;

            // Despawn existing with same key to avoid duplicates/leaks
            Despawn(key);

            // Load via ResourceManager
            var resources = Engine.Instance.GetService<ResourceManager>();
            if (resources == null) return;

            // Paths: Try "VFX/Name", then "Prefabs/Name"
            GameObject prefab = await resources.LoadAssetAsync<GameObject>($"VFX/{prefabName}");
            if (!prefab) prefab = await resources.LoadAssetAsync<GameObject>($"Prefabs/{prefabName}");

            if (prefab)
            {
                var instance = Instantiate(prefab, VFXRoot);
                instance.name = key;

                var rect = instance.GetComponent<RectTransform>();
                if (rect)
                {
                    rect.anchoredPosition = position;
                    rect.localScale = scale;
                }
                else
                {
                    instance.transform.localPosition = position;
                    instance.transform.localScale = scale;
                }

                _spawnedObjects[key] = instance;
            }
            else
            {
                Debug.LogWarning($"[SpawnManager] Could not find prefab: {prefabName}");
            }
        }

        public void Despawn(string key)
        {
            if (_spawnedObjects.TryGetValue(key, out var obj))
            {
                if (obj) Destroy(obj);
                _spawnedObjects.Remove(key);
            }
        }

        public void DespawnAll()
        {
            foreach (var kvp in _spawnedObjects)
            {
                if (kvp.Value) Destroy(kvp.Value);
            }
            _spawnedObjects.Clear();
        }
    }
}
