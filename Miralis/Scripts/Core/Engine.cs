using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VSNL.State;
using VSNL.Characters;
using VSNL.Services;
using VSNL.UI; // For BacklogManager
using VSNLEngine.Core; // For ScriptPlayer refactor

namespace VSNL.Core
{
    /// <summary>
    /// The core Engine class. Acts as key Service Locator and Bootstrapper.
    /// Persists across scenes.
    /// </summary>
    public class Engine : MonoBehaviour
    {
        public static Engine Instance { get; private set; }
        
        [Header("Configuration")]
        [Tooltip("Name of the VSNL script to load on start (in Resources/Scripts/)")]
        public string InitialScriptName = "Start";

        private Dictionary<Type, IGameService> _services = new Dictionary<Type, IGameService>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            await InitializeEngineAsync();
        }

        private async UniTask InitializeEngineAsync()
        {
            Debug.Log("[VSNL Engine] Initializing...");

            // Register Services
            // Note: Order matters for dependencies
            
            // Register Services using helper to find existing configured instances
            RegisterService(FindOrAddService<ResourceManager>());
            RegisterService(FindOrAddService<LocalizationManager>());
            RegisterService(FindOrAddService<SaveLoadManager>());
            RegisterService(FindOrAddService<VariableManager>());
            RegisterService(FindOrAddService<ScriptPlayer>());
            RegisterService(FindOrAddService<UIManager>());
            RegisterService(FindOrAddService<BacklogManager>());
            RegisterService(FindOrAddService<CharacterManager>());
            RegisterService(FindOrAddService<AudioManager>());
            RegisterService(FindOrAddService<BackgroundManager>());
            RegisterService(FindOrAddService<VSNL.Media.VideoManager>());
            RegisterService(FindOrAddService<InputService>());

            // Initialize all registered services
            foreach (var service in _services.Values)
            {
                await service.InitializeAsync();
            }

            Debug.Log("[VSNL Engine] Initialization Complete.");

            // Optional: Auto-start a debug script if found
            if (!string.IsNullOrEmpty(InitialScriptName))
            {
                var startScript = Resources.Load<TextAsset>($"Scripts/{InitialScriptName}");
                Debug.Log($"[VSNL Engine] Attempting to load 'Scripts/{InitialScriptName}': {(startScript ? "Found" : "Not Found")}");
                
                if (startScript)
                {
                    Debug.Log($"[VSNL Engine] Auto-starting '{InitialScriptName}'...");
                    var player = GetService<ScriptPlayer>();
                    player.LoadScript(startScript);
                    player.PlayScriptAsync().Forget();
                }
            }
        }

        /// <summary>
        /// Registers a service instance.
        /// </summary>
        public void RegisterService<T>(T service) where T : IGameService
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[VSNL Engine] Service {type.Name} is already registered.");
                return;
            }
            _services.Add(type, service);
        }

        /// <summary>
        /// Retrieves a registered service.
        /// </summary>
        public T GetService<T>() where T : class, IGameService
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return service as T;
            }
            Debug.LogError($"[VSNL Engine] Service {type.Name} not found!");
            return null;
        }

        private T FindOrAddService<T>() where T : Component, IGameService
        {
            // 1. Check self
            T service = GetComponent<T>();
            if (service) return service;

            // 2. Check children (useful if user organized managers under Engine)
            service = GetComponentInChildren<T>(true);
            if (service) return service;

            // 3. Check global scene (useful if managers are separate root objects)
            service = FindFirstObjectByType<T>();
            if (service)
            {
                // Optional: Parenting to ensure DontDestroyOnLoad works if checking root
                // For now, let's respect user hierarchy but warn if not persistent?
                // Or just auto-parent:
                // if (service.transform.parent == null) service.transform.SetParent(transform);
                return service;
            }

            // 4. Create new on self (Fallback)
            return gameObject.AddComponent<T>();
        }

        public async UniTask RestoreState(GameState state)
        {
            Debug.Log("[Engine] Restoring State...");
            
            // 1. Audio
            var audio = GetService<AudioManager>();
            if (audio != null)
            {
                if (!string.IsNullOrEmpty(state.CurrentMusic)) audio.PlayBGM(state.CurrentMusic);
                else audio.StopBGM();
            }

            // 2. Background
            var bg = GetService<BackgroundManager>();
            if (bg != null && !string.IsNullOrEmpty(state.CurrentBackground))
            {
                await bg.SetBackgroundAsync(state.CurrentBackground);
            }

            // 3. Characters
            var chars = GetService<CharacterManager>();
            if (chars != null)
            {
                // Clear existing (API needed) -> simplistic approach: just show active ones
                // Ideally CharacterManager should have a 'Reset' or 'Restore' but we can iterate
                foreach (var c in state.ActiveCharacters)
                {
                   if (c.IsVisible) 
                   {
                       // We need to support Position restoration. For now we use the ShowCharacterAsync.
                       // Ideally split position setting. But Show usually handles it.
                       await chars.ShowCharacterAsync(c.Name, c.Emotion, c.Position);
                   }
                }
            }

            // 4. Script Execution
            var scriptPlayer = GetService<ScriptPlayer>();
            if (scriptPlayer != null)
            {
                // Load script if different
                // Note: This logic assumes script is in Resources/Scripts/NAME
                var scriptAsset = Resources.Load<TextAsset>($"Scripts/{state.ScriptFileName}");
                if (scriptAsset)
                {
                    scriptPlayer.LoadScript(scriptAsset);
                    scriptPlayer.JumpToLine(state.LineIndex);
                    // Resume execution? Usually Load pauses gameplay or waits for user.
                    // But if we want to seamless load, we might need to Auto-Resume.
                    // Let's NOT auto-resume for now, let caller decide, or StartCoroutine.
                }
            }
            
            Debug.Log("[Engine] State Restored.");
        }
    }
}
