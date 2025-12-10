using UnityEngine;

namespace VSNL.UI.Utility
{
    /// <summary>
    /// Adjusts RectTransform to respect Screen.safeArea (for mobile notches).
    /// Attach this to a "SafeAreaContainer" immediate child of Canvas.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
        private Vector2Int _lastScreenSize = new Vector2Int(0, 0);

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            Refresh();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea || _lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            _lastSafeArea = Screen.safeArea;
            _lastScreenSize.x = Screen.width;
            _lastScreenSize.y = Screen.height;

            ApplySafeArea(Screen.safeArea);
        }

        private void ApplySafeArea(Rect r)
        {
            // Convert Safe Area Rect to Anchor Min/Max
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
            {
                _rectTransform.anchorMin = anchorMin;
                _rectTransform.anchorMax = anchorMax;
            }
        }
    }
}
