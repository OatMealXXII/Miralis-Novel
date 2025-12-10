#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VSNL.Core.Data;
using System.Collections.Generic;

namespace VSNL.EditorTools
{
    public class CharacterDatabaseEditor : EditorWindow
    {
        private CharacterMetadata _targetMetadata;
        private Vector2 _scrollPos;

        [MenuItem("VSNL/Character Database")]
        public static void ShowWindow()
        {
            GetWindow<CharacterDatabaseEditor>("Char Database");
        }

        private void OnGUI()
        {
            GUILayout.Label("Character Metadata Editor", EditorStyles.boldLabel);

            _targetMetadata = (CharacterMetadata)EditorGUILayout.ObjectField("Database", _targetMetadata, typeof(CharacterMetadata), false);

            if (_targetMetadata == null)
            {
                GUILayout.Label("Assign or Create a Metadata Asset.");
                if (GUILayout.Button("Create New Metadata Asset"))
                {
                    CreateAsset();
                }
                return;
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (_targetMetadata.Characters == null) _targetMetadata.Characters = new List<CharacterMetadata.CharacterData>();

            for (int i = 0; i < _targetMetadata.Characters.Count; i++)
            {
                DrawCharacterGui(i);
                GUILayout.Space(10);
            }

            if (GUILayout.Button("Add New Character"))
            {
                _targetMetadata.Characters.Add(new CharacterMetadata.CharacterData { CharacterID = "NewID" });
                EditorUtility.SetDirty(_targetMetadata);
            }

            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_targetMetadata);
            }
        }

        private void DrawCharacterGui(int index)
        {
            var charData = _targetMetadata.Characters[index];
            
            GUILayout.BeginVertical("box");
            
            GUILayout.BeginHorizontal();
            charData.CharacterID = EditorGUILayout.TextField("ID", charData.CharacterID);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _targetMetadata.Characters.RemoveAt(index);
                return; 
            }
            GUILayout.EndHorizontal();

            charData.DisplayName = EditorGUILayout.TextField("Display Name", charData.DisplayName);
            charData.NameColor = EditorGUILayout.ColorField("Name Color", charData.NameColor);

            GUILayout.Label("Emotions:");
            if (charData.Emotions == null) charData.Emotions = new List<CharacterMetadata.EmotionData>();

            for (int j = 0; j < charData.Emotions.Count; j++)
            {
                var emotion = charData.Emotions[j];
                GUILayout.BeginHorizontal();
                emotion.EmotionName = EditorGUILayout.TextField(emotion.EmotionName, GUILayout.Width(100));
                emotion.SpriteAsset = (Sprite)EditorGUILayout.ObjectField(emotion.SpriteAsset, typeof(Sprite), false);
                
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    charData.Emotions.RemoveAt(j);
                    break;
                }
                charData.Emotions[j] = emotion; // structs are value types, reassign
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Emotion"))
            {
                charData.Emotions.Add(new CharacterMetadata.EmotionData { EmotionName = "Normal" });
            }

            GUILayout.EndVertical();
        }

        private void CreateAsset()
        {
            var asset = CreateInstance<CharacterMetadata>();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/CharacterDatabase.asset");
            AssetDatabase.SaveAssets();
            _targetMetadata = asset;
        }
    }
}
#endif
