using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;

[ScriptedImporter(1, "vsnl")]
public class VSNLImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        string text = File.ReadAllText(ctx.assetPath);
        TextAsset assets = new TextAsset(text);
        ctx.AddObjectToAsset("main", assets);
        ctx.SetMainObject(assets);
    }
}
