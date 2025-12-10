using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VSNL.Core;
using System;

namespace VSNL.Services
{
    public class TransitionManager : MonoBehaviour, IGameService
    {
        public CanvasGroup FadeOverlay;
        public float DefaultDuration = 0.5f;

        public async UniTask InitializeAsync()
        {
            // Auto-create Overlay
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas && !FadeOverlay)
            {
                var obj = new GameObject("TransitionOverlay");
                obj.transform.SetParent(canvas.transform, false);
                obj.transform.SetAsLastSibling(); // Top

                var rect = obj.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                var img = obj.AddComponent<Image>();
                img.color = Color.black;
                img.raycastTarget = false; 

                FadeOverlay = obj.AddComponent<CanvasGroup>();
                FadeOverlay.alpha = 0f;
            }

            Debug.Log("[TransitionManager] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            if (FadeOverlay)
            {
                FadeOverlay.DOKill();
                FadeOverlay.alpha = 0f;
            }
        }

        public async UniTask FadeOutAsync(float duration = -1)
        {
            if (duration < 0) duration = DefaultDuration;
            await FadeToAsync(1f, duration);
        }

        public async UniTask FadeInAsync(float duration = -1)
        {
            if (duration < 0) duration = DefaultDuration;
            await FadeToAsync(0f, duration);
        }

        public async UniTask TransitionAsync(Action midAction, float duration = -1)
        {
            await FadeOutAsync(duration);
            midAction?.Invoke();
            await UniTask.Delay(100); 
            await FadeInAsync(duration);
        }

        private async UniTask FadeToAsync(float targetAlpha, float duration)
        {
            if (!FadeOverlay) return;

            FadeOverlay.blocksRaycasts = true; 
            
            // Use DOTween
            await FadeOverlay.DOFade(targetAlpha, duration)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();

            FadeOverlay.blocksRaycasts = (targetAlpha > 0);
        }
    }
}
