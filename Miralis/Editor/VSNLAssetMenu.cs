using UnityEngine;
using UnityEditor;
using System.IO;

namespace VSNL.Editor
{
    public class VSNLAssetMenu
    {
        [MenuItem("Assets/Create/VSNL Script", false, 80)]
        public static void CreateVSNLScript()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (Directory.Exists(path))
            {
                // Path is valid directory
            }
            else
            {
                // Path is a file, get parent
                path = Path.GetDirectoryName(path);
            }

            string defaultName = "NewScript.vsnl";
            string fullPath = AssetDatabase.GenerateUniqueAssetPath(path + "/" + defaultName);
            
            string templateContent = @"[Start]
# Write your VSNL script here
@log ""Hello VSNL!""
";

            File.WriteAllText(fullPath, templateContent);
            AssetDatabase.Refresh();
            
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
            Selection.activeObject = asset;
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}
