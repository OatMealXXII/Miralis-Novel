#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VSNL.Core;
using VSNL.UI;

public class MiralisMenu : MonoBehaviour
{
    [MenuItem("VSNL/Create Game Scene")]
    public static void CreateGameScene()
    {
        // 1. Create Engine Object
        var engineObj = new GameObject("VSNL_Engine");
        var engine = engineObj.AddComponent<Engine>();
        
        // 2. Create Canvas
        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // 3. Create UI Structure
        // Background Layer
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        bgObj.AddComponent<UnityEngine.UI.Image>().color = Color.black;
        
        // Character Layer
        var charRoot = new GameObject("CharacterRoot");
        charRoot.transform.SetParent(canvasObj.transform, false);
        var charRect = charRoot.AddComponent<RectTransform>();
        charRect.anchorMin = Vector2.zero;
        charRect.anchorMax = Vector2.one;
        charRect.offsetMin = Vector2.zero;
        charRect.offsetMax = Vector2.zero;
        
        // Dialogue Panel
        var panelObj = new GameObject("DialoguePanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        var panelImg = panelObj.AddComponent<UnityEngine.UI.Image>();
        panelImg.color = new Color(0, 0, 0, 0.8f);
        
        var panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0);
        panelRect.anchorMax = new Vector2(0.9f, 0.3f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Speaker Text
        var speakerObj = new GameObject("SpeakerName");
        speakerObj.transform.SetParent(panelObj.transform, false);
        var speakerText = speakerObj.AddComponent<TMPro.TextMeshProUGUI>();
        speakerText.text = "Speaker";
        speakerText.fontSize = 32;
        speakerText.alignment = TMPro.TextAlignmentOptions.TopLeft;
        
        var speakerRect = speakerObj.GetComponent<RectTransform>();
        speakerRect.anchorMin = new Vector2(0, 1);
        speakerRect.anchorMax = new Vector2(1, 1);
        speakerRect.offsetMin = new Vector2(20, -50);
        speakerRect.offsetMax = new Vector2(-20, -10);

        // Dialogue Text
        var textObj = new GameObject("DialogueText");
        textObj.transform.SetParent(panelObj.transform, false);
        var diaText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        diaText.text = "Dialogue goes here...";
        diaText.fontSize = 28;
        
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(25, 20);
        textRect.offsetMax = new Vector2(-25, -60);
        
        // Add Typer
        var typer = textObj.AddComponent<RichTextTyper>();
        
        // Link UI to Engine/Services (Implicitly by naming or Setup)
        // Ideally we should set the references in the Engine or Managers via Inspector if possible.
        // But our Engine registers services dynamically. 
        // The UIManager needs references.
        
        var uiManager = engineObj.GetComponent<UIManager>(); // It's not added yet, adds on Runtime.
        // But we can add a helper "Setup" script or manually assign refs if we add components now.
        // For simplicity, let's add UIManager now so we can link it.
        uiManager = engineObj.AddComponent<UIManager>();
        uiManager.DialoguePanel = panelObj;
        uiManager.SpeakerText = speakerText;
        uiManager.DialogueText = diaText;
        // uiManager.TextManager = ... (it finds it on Panel or self)

        // Choice Panel
        var choicePanel = new GameObject("ChoicePanel");
        choicePanel.transform.SetParent(canvasObj.transform, false);
        choicePanel.AddComponent<UnityEngine.UI.Image>().color = new Color(0,0,0,0.5f);
        choicePanel.SetActive(false);
        var choiceRect = choicePanel.AddComponent<RectTransform>();
        choiceRect.anchorMin = Vector2.zero;
        choiceRect.anchorMax = Vector2.one;
        
        var layout = choicePanel.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 20;

        uiManager.ChoicePanel = choicePanel;
        uiManager.ChoiceContainer = choicePanel.transform;
        
        // Create a simple Button Prefab usage?
        // We need a prefab for buttons.
        // For now, let's create a basic one in scene to duplicate? 
        // Or just let user handle it.
        // Let's create a dummy button as prefab and hide it.
        var btnTemplate = new GameObject("ButtonTemplate");
        btnTemplate.transform.SetParent(choicePanel.transform, false);
        btnTemplate.AddComponent<UnityEngine.UI.Image>().color = Color.white;
        var btnCmp = btnTemplate.AddComponent<UnityEngine.UI.Button>();
        
        var btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnTemplate.transform, false);
        var btnText = btnTextObj.AddComponent<TMPro.TextMeshProUGUI>();
        btnText.text = "Option";
        btnText.color = Color.black;
        btnText.alignment = TMPro.TextAlignmentOptions.Center;
        
        // Link to UIManager
        uiManager.ChoiceButtonPrefab = btnTemplate;
        // But ButtonTemplate needs ChoiceButton component
        var choiceBtn = btnTemplate.AddComponent<ChoiceButton>();
        choiceBtn.ButtonComponent = btnCmp;
        choiceBtn.TextComponent = btnText;
        
        // Hide template
        btnTemplate.SetActive(false); // Prefab in scene

        Selection.activeGameObject = engineObj;
        Debug.Log("[VSNL] Created Game Scene Setup.");
    }
}
#endif
