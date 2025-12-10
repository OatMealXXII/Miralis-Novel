using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using VSNL.Core;

namespace VSNL.Media
{
    public class VideoManager : MonoBehaviour, IGameService
    {
        public VideoPlayer VideoPlayer;
        public RawImage DisplayImage;
        public Canvas CanvasRef;

        private RenderTexture _renderTexture;
        private bool _isPlayingBlocking;

        public async UniTask InitializeAsync()
        {
            // 1. Find Canvas
            if (!CanvasRef) CanvasRef = FindFirstObjectByType<Canvas>(); // Main canvas

            // 2. Setup Display
            if (CanvasRef && !DisplayImage)
            {
                GameObject vidObj = new GameObject("VideoDisplay");
                vidObj.transform.SetParent(CanvasRef.transform, false);
                // Place above background (Index 1). Background is usually Index 0.
                vidObj.transform.SetSiblingIndex(1); 

                var rect = vidObj.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                DisplayImage = vidObj.AddComponent<RawImage>();
                DisplayImage.color = Color.white;
                DisplayImage.raycastTarget = false; // Let clicks pass through if background
                DisplayImage.enabled = false; // Hide initially
            }

            // 3. Setup Video Player
            if (!VideoPlayer)
            {
                VideoPlayer = gameObject.AddComponent<VideoPlayer>();
                VideoPlayer.playOnAwake = false;
                VideoPlayer.isLooping = false;
                VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            }

            Debug.Log("[VideoManager] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            StopVideo();
        }

        public async UniTask PlayVideoAsync(string fileName, bool isBlocking, bool isLooped)
        {
            var resources = Engine.Instance.GetService<ResourceManager>();
            // Note: VideoPlayer usually needs a direct URL or Clip. 
            // Resources.Load<VideoClip> works.
            // Addressables also works.
            // We'll trust ResourceManager returns a VideoClip (we need to support loading VideoClip).
            // But ResourceManager LoadAssetAsync<T> checks T : Object. VideoClip is Object.
            
            // Path assumption: "Videos/" + fileName
            string path = $"Videos/{fileName}";
            
            // We rely on ResourceManager to load the clip.
            // NOTE: VideoPlayer works best with URL for streaming, but for VN short clips, VideoClip is fine.
            VideoClip clip = null;
            if (resources != null)
            {
                clip = await resources.LoadAssetAsync<VideoClip>(path);
            }

            if (clip == null)
            {
                Debug.LogWarning($"[VideoManager] Could not load video: {path}");
                return;
            }

            // Prepare Render Texture
            if (_renderTexture == null)
            {
                _renderTexture = new RenderTexture(1920, 1080, 0);
            }
            
            VideoPlayer.source = VideoSource.VideoClip;
            VideoPlayer.clip = clip;
            VideoPlayer.targetTexture = _renderTexture;
            VideoPlayer.isLooping = isLooped;

            if (DisplayImage)
            {
                DisplayImage.texture = _renderTexture;
                DisplayImage.enabled = true;
                
                // If blocking, ensure we catch clicks?
                // Actually, UIManager usually handles clicks. 
                // If blocking, valid click = Skip.
                // We'll handle input in the loop.
            }

            _isPlayingBlocking = isBlocking;
            var ui = Engine.Instance.GetService<UIManager>();

            if (isBlocking && ui != null)
            {
                ui.HideDialogue();
            }

            VideoPlayer.Prepare();
            while (!VideoPlayer.isPrepared) await UniTask.Yield();

            VideoPlayer.Play();

            if (isBlocking)
            {
                // Wait loop
                while (VideoPlayer.isPlaying)
                {
                    // Check Skip
                    if (Input.GetMouseButtonDown(0))
                    {
                        VideoPlayer.Stop();
                        break;
                    }
                    await UniTask.Yield();
                }

                // Cleanup
                DisplayImage.enabled = false;
                DisplayImage.texture = null;
                
                if (ui != null)
                {
                    // Restore UI? Only if we were showing it? 
                    // Usually yes, blocking movie implies a cutscene inside a script.
                    // But maybe we were already hidden?
                    // We'll show it to be safe, or script should handle it.
                    // VSNL spec: @movie blocking -> usually resumes context.
                    // Let's NOT auto-show, but if user had text, next line will show it.
                    // But if we hid it, we should probably restore state.
                    // Let's assume script continues.
                }
            }
            else
            {
                // Background mode: let it run.
            }
        }

        public void StopVideo()
        {
            if (VideoPlayer) VideoPlayer.Stop();
            if (DisplayImage) 
            {
                DisplayImage.enabled = false;
                DisplayImage.texture = null;
            }
            if (_renderTexture) _renderTexture.Release();
        }
    }
}

