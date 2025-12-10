#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VSNL.Editor.Graph
{
    public class VSNLGraphWindow : EditorWindow
    {
        private VSNLGraphView _graphView;
        private string _fileName = "NewStory";

        [MenuItem("VSNL/Graph Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<VSNLGraphWindow>();
            window.titleContent = new GUIContent("VSNL Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        private void ConstructGraphView()
        {
            _graphView = new VSNLGraphView();
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new UnityEditor.UIElements.Toolbar();

            var fileNameField = new TextField("File Name:");
            fileNameField.SetValueWithoutNotify(_fileName);
            fileNameField.MarkDirtyRepaint();
            fileNameField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameField);

            var saveButton = new UnityEngine.UIElements.Button(() => Save()) { text = "Export to VSNL" };
            toolbar.Add(saveButton);

            rootVisualElement.Add(toolbar);
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_fileName)) 
            {
                EditorUtility.DisplayDialog("Error", "Please enter a file name.", "OK");
                return;
            }

            var path = $"Assets/Resources/Scripts/{_fileName}.txt"; // VSNL uses .txt or .vsnl
            VSNLGraphExporter.Export(_graphView, path);
            AssetDatabase.Refresh();
        }
    }
}
#endif
