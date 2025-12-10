using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using VSNL.Core;

using VSNL.Core.Data;

namespace VSNL.Characters
{
    public class CharacterManager : MonoBehaviour, IGameService
    {
        [Header("Configuration")]
        public CharacterMetadata MetadataDB; // Assign in Inspector
        public Transform CharacterRoot; // Container for characters
        public GameObject CharacterPrefab; // Optional: If null, will create dynamic Image

        private Dictionary<string, CharacterView> _activeCharacters = new Dictionary<string, CharacterView>();

        public async UniTask InitializeAsync()
        {
            Debug.Log("[CharacterManager] Initialized.");
            
            // Create root if needed
            if (!CharacterRoot)
            {
                var canvas = FindFirstObjectByType<Canvas>();
                if (canvas)
                {
                    var rootObj = new GameObject("CharacterRoot");
                    rootObj.transform.SetParent(canvas.transform, false);
                    rootObj.AddComponent<RectTransform>().anchorMin = Vector2.zero;
                    rootObj.AddComponent<RectTransform>().anchorMax = Vector2.one;
                    rootObj.AddComponent<RectTransform>().offsetMin = Vector2.zero;
                    rootObj.AddComponent<RectTransform>().offsetMax = Vector2.zero;
                    
                    // Ensure it's behind loading/UI but in front of BG (if we had layers)
                    // For now, simple creation
                    CharacterRoot = rootObj.transform;
                }
            }

            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            foreach (var charView in _activeCharacters.Values)
            {
                if (charView) Destroy(charView.gameObject);
            }
            _activeCharacters.Clear();
        }

        public async UniTask ShowCharacterAsync(string name, string emotion, string position)
        {
            CharacterView view;

            // 1. Get or Create View
            if (_activeCharacters.ContainsKey(name))
            {
                view = _activeCharacters[name];
            }
            else
            {
                view = CreateCharacterView(name);
                _activeCharacters.Add(name, view);
            }

            // 2. Load Sprite / Prefab
            Sprite sprite = null;
            GameObject prefab = null;

            // A. Try Metadata Lookup
            if (MetadataDB)
            {
                var data = MetadataDB.GetCharacter(name);
                if (data != null)
                {
                    sprite = data.GetSprite(emotion);
                    prefab = data.Live2DModelPrefab; // Check for overall model
                    
                    if (sprite) Debug.Log($"[CharacterManager] Loaded Sprite from Metadata: {name}:{emotion}");
                }
            }

            // B. Fallback to Resources (Legacy) for Sprites
            if (sprite == null && prefab == null)
            {
                var resources = Engine.Instance.GetService<ResourceManager>();
                if (resources != null)
                {
                     string path1 = $"Characters/{name}/{emotion}";
                     string path2 = $"Characters/{name}_{emotion}";
                     
                     sprite = await resources.LoadAssetAsync<Sprite>(path1);
                     if (!sprite)
                     {
                         sprite = await resources.LoadAssetAsync<Sprite>(path2);
                     }
                     
                     if (!sprite)
                     {
                         Debug.LogWarning($"[CharacterManager] Failed to load sprite for {name}:{emotion} at {path1} or {path2}");
                     }
                }
            }
            
            // Logic: If view already has Live2D model, just update expression. 
            // If not, instantiate it.
            // Simplified: If Prefab exists in metadata, ensure View has it.
            
            if (prefab)
            {
                view.SetLive2DModel(prefab);
                // Set Expression
                view.SetExpression(emotion);
            }
            else if (sprite)
            {
                view.SetSprite(sprite);
            }
            else
            {
                Debug.LogWarning($"[CharacterManager] Asset not found for {name} ({emotion})");
            }

            // 3. Set Position
            if (!string.IsNullOrEmpty(position))
            {
                view.SetPosition(position, 0.5f);
            }
            
            await UniTask.CompletedTask;
        }

        public CharacterView GetCharacterView(string name)
        {
             if (_activeCharacters.TryGetValue(name, out var view)) return view;
             return null;
        }

        private CharacterView CreateCharacterView(string name)
        {
            GameObject go;
            // Always use Prefab/Dynamic logic:
            if (CharacterPrefab)
            {
                go = Instantiate(CharacterPrefab, CharacterRoot);
            }
            else
            {
                go = new GameObject($"Char_{name}");
                go.transform.SetParent(CharacterRoot, false);
                // Ensure Image component exists for Sprite fallback
                go.AddComponent<Image>();
            }

            var view = go.GetComponent<CharacterView>();
            if (!view) view = go.AddComponent<CharacterView>();
            
            view.Setup(name);
            return view;
        }
    }
}
