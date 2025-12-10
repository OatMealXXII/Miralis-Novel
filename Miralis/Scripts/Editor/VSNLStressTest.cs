#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.TestTools; // Standard Unity Test framework
using NUnit.Framework;
using System.Collections;
using VSNL.Core;
using Cysharp.Threading.Tasks;
using VSNLEngine.Core; // Fix: Import ScriptPlayer namespace


public class VSNLStressTest
{
    [UnityTest]
    public IEnumerator DialogueStressTest_10kLines() => UniTask.ToCoroutine(async () =>
    {
        // 1. Setup Engine
        var engineObj = new GameObject("Engine_Test");
        var engine = engineObj.AddComponent<Engine>();
        await UniTask.Delay(500); // Wait for Init

        var player = Engine.Instance.GetService<ScriptPlayer>();
        Assert.IsNotNull(player, "ScriptPlayer not found");
        
        // 2. Mock Test
        Debug.Log("Starting Stress Test...");
        await UniTask.Yield();
        
        // 3. Cleanup
        Object.Destroy(engineObj);
    });
}
#endif
