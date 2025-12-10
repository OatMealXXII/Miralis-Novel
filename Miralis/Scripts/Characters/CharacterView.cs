using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace VSNL.Characters
{
    [RequireComponent(typeof(Image))]
    public class CharacterView : MonoBehaviour
    {
        public string CharacterName { get; private set; }
        public Image SpriteHandler;

        private RectTransform _rectTransform;

        private void Awake()
        {
            SpriteHandler = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
            SpriteHandler.preserveAspect = true;
        }

        public void Setup(string name)
        {
            CharacterName = name;
            gameObject.name = $"Char_{name}";
        }

        public VSNL.Characters.Live2D.Live2DController Live2DController { get; private set; }

        public void SetSprite(Sprite sprite)
        {
            if (Live2DController) 
            {
                 // Destroy Live2D if switching back to sprite?
                 // Simple implementation: Disable or Destroy
                 Destroy(Live2DController.gameObject); 
                 Live2DController = null;
            }

            if (SpriteHandler)
            {
                SpriteHandler.enabled = true;
                SpriteHandler.sprite = sprite;
                SpriteHandler.SetNativeSize(); 
            }
        }

        public void SetLive2DModel(GameObject prefab)
        {
            if (SpriteHandler) SpriteHandler.enabled = false; // Hide 2D sprite
            
            // Check if already instantiated
            if (Live2DController != null) return; // Assume same model for now

            var go = Instantiate(prefab, transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one; // Adjust if needed
            
            Live2DController = go.GetComponent<VSNL.Characters.Live2D.Live2DController>();
            if (!Live2DController) Live2DController = go.AddComponent<VSNL.Characters.Live2D.Live2DController>();
        }

        public void SetExpression(string expression)
        {
            if (Live2DController) Live2DController.SetExpression(expression);
        }

        public void PlayMotion(string motion)
        {
             if (Live2DController) Live2DController.PlayMotion(motion);
        }

        public async void SetPosition(string positionName, float duration = 0.5f)
        {
            // Simple anchor positions
            // Assuming Parent is a full-screen Canvas/Panel
            
            float xPos = 0;
            switch (positionName.ToLower())
            {
                case "left": xPos = -400; break;
                case "right": xPos = 400; break;
                case "center": xPos = 0; break;
                // Add more custom parsing if needed
            }

            // Kill any existing tweens
            _rectTransform.DOKill();
            
            if (duration > 0)
            {
                await _rectTransform.DOAnchorPosX(xPos, duration)
                    .SetEase(Ease.OutQuad)
                    .AsyncWaitForCompletion();
            }
            else
            {
                _rectTransform.anchoredPosition = new Vector2(xPos, _rectTransform.anchoredPosition.y);
            }
        }
    }
}
