using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace VSNL.DebugSystem
{
    public class DebugConsoleUI : MonoBehaviour
    {
        private DebugConsole _service;
        private Canvas _canvas;
        private TMP_InputField _inputField;
        private TextMeshProUGUI _logText;
        private ScrollRect _scrollRect;
        private GameObject _panel;

        public void Initialize(DebugConsole service)
        {
            _service = service;
            BuildUI();
        }

        public void SetVisible(bool visible)
        {
            if (_panel) _panel.SetActive(visible);
            if (visible && _inputField) 
            {
                _inputField.ActivateInputField();
                _inputField.Select();
            }
        }

        public void AppendLog(string message)
        {
            if (_logText) _logText.text += message + "\n";
            // Auto scroll? simple hack
            if (_scrollRect) Canvas.ForceUpdateCanvases();
            if (_scrollRect) _scrollRect.verticalNormalizedPosition = 0f;
        }

        private void BuildUI()
        {
            // Runtime UI Generation for "Wow factor" reliability
            var go = this.gameObject;
            _canvas = go.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 999; // Top most
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            go.AddComponent<GraphicRaycaster>();

            // Panel half screen
            _panel = CreateObj("ConsolePanel", go.transform);
            var img = _panel.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.8f);
            var rect = _panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Scroll View
            var scrollObj = CreateObj("ScrollView", _panel.transform);
            var svRect = scrollObj.GetComponent<RectTransform>();
            svRect.anchorMin = new Vector2(0, 0.15f); // Leave space for input
            svRect.anchorMax = new Vector2(1, 1);
            svRect.offsetMin = new Vector2(10, 10);
            svRect.offsetMax = new Vector2(-10, -10);
            _scrollRect = scrollObj.AddComponent<ScrollRect>();
            
            // Viewport
            var vp = CreateObj("Viewport", scrollObj.transform);
            var vpRect = vp.GetComponent<RectTransform>();
            vpRect.anchorMin = Vector2.zero; vpRect.anchorMax = Vector2.one;
            vp.AddComponent<RectMask2D>();
            
            // Content
            var content = CreateObj("Content", vp.transform);
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1); contentRect.anchorMax = Vector2.one;
            contentRect.pivot = new Vector2(0.5f, 1);
            var vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.childControlHeight = true; vlg.childForceExpandHeight = false;
            var csf = content.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            _scrollRect.content = contentRect;
            _scrollRect.viewport = vpRect;

            // Log Text
            var textObj = CreateObj("LogText", content.transform);
            _logText = textObj.AddComponent<TextMeshProUGUI>();
            _logText.fontSize = 20;
            _logText.color = Color.green;

            // Input Field
            var inputObj = CreateObj("InputField", _panel.transform);
            var inRect = inputObj.GetComponent<RectTransform>();
            inRect.anchorMin = Vector2.zero; 
            inRect.anchorMax = new Vector2(1, 0.1f);
            inputObj.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Input Text Area
            var textArea = CreateObj("TextArea", inputObj.transform);
            var taRect = textArea.GetComponent<RectTransform>();
            taRect.anchorMin = Vector2.zero; taRect.anchorMax = Vector2.one;
            taRect.offsetMin = new Vector2(10, 5); taRect.offsetMax = new Vector2(-10, -5);
            
            // Actual Input Text
            var inTextObj = CreateObj("Text", textArea.transform);
            var inText = inTextObj.AddComponent<TextMeshProUGUI>();
            inText.fontSize = 24;
            inText.color = Color.white;
            
            _inputField = inputObj.AddComponent<TMP_InputField>();
            _inputField.textViewport = taRect;
            _inputField.textComponent = inText;
            _inputField.onSubmit.AddListener(OnSubmit);
        }

        private GameObject CreateObj(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }

        private void OnSubmit(string val)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                _service.ExecuteCommand(val);
                _inputField.text = "";
                _inputField.ActivateInputField(); // Keep focus
            }
        }
    }
}
