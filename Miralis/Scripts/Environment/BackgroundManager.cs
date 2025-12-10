using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using VSNL.Core;

namespace VSNL.Services
{
    public class BackgroundManager : MonoBehaviour, IGameService
    {
        public Image BackgroundImage;
        public Canvas CanvasRef;

        public async UniTask InitializeAsync()
        {
            // Auto-create BG UI if missing
            if (CanvasRef == null)
            {
                CanvasRef = FindFirstObjectByType<Canvas>(); // Naive find, assuming one main canvas
            }

            if (CanvasRef != null && BackgroundImage == null)
            {
                // Create a BG object at the back
                GameObject bgObj = new GameObject("Background");
                bgObj.transform.SetParent(CanvasRef.transform, false);
                bgObj.transform.SetAsFirstSibling(); // Push to back
                
                var rect = bgObj.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                BackgroundImage = bgObj.AddComponent<Image>();
                // Set default color or sprite?
                BackgroundImage.color = Color.black; 
            }

            Debug.Log("[BackgroundManager] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            if (BackgroundImage) BackgroundImage.sprite = null;
        }

        private string _currentBgPath;

        public async UniTask SetBackgroundAsync(string imageName)
        {
            // Load via ResourceManager
            var resources = Engine.Instance.GetService<ResourceManager>();
            Sprite sprite = null;
            
            // Unload previous
            if (!string.IsNullOrEmpty(_currentBgPath) && resources != null)
            {
                resources.UnloadAsset(_currentBgPath);
                _currentBgPath = null;
            }

            if (resources != null)
            {
                string path = $"Backgrounds/{imageName}";
                sprite = await resources.LoadAssetAsync<Sprite>(path);
                if (sprite)
                {
                    _currentBgPath = path;
                }
                else
                {
                    path = $"Images/{imageName}";
                    sprite = await resources.LoadAssetAsync<Sprite>(path);
                    if (sprite) _currentBgPath = path;
                }
            }

            var transitions = Engine.Instance.GetService<TransitionManager>();
            
            if (transitions != null && BackgroundImage.sprite != null) 
            {
                await transitions.TransitionAsync(() => 
                {
                     if (BackgroundImage) 
                     {
                         if (sprite == null && !string.IsNullOrEmpty(imageName)) Debug.LogWarning($"[BackgroundManager] Missing background: {imageName}");
                         BackgroundImage.sprite = sprite; 
                         // If null (missing), it effectively clears BG, but safely.
                     }
                });
            }
            else
            {
                if (BackgroundImage) 
                {
                    if (sprite == null && !string.IsNullOrEmpty(imageName)) Debug.LogWarning($"[BackgroundManager] Missing background: {imageName}");
                    BackgroundImage.sprite = sprite;
                }
                if (transitions != null && sprite != null) await transitions.FadeInAsync(); 
            }
            
            await UniTask.CompletedTask;
        }
    }
}
