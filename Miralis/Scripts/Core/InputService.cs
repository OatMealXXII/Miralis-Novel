using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;

namespace VSNL.Core
{
    public class InputService : MonoBehaviour, IGameService
    {
        public InputActionAsset InputActionAsset; // Optional: If user provides one
        
        // Code-based Actions
        public InputAction SubmitAction { get; private set; }
        public InputAction CancelAction { get; private set; }
        public InputAction SkipAction { get; private set; }
        public InputAction HideAction { get; private set; }
        public InputAction LogAction { get; private set; }
        public InputAction MenuAction { get; private set; }
        public InputAction NavigateAction { get; private set; } // D-Pad/Arrows

        // Events
        public event Action OnSubmit;
        public event Action OnCancel;
        public event Action<bool> OnSkip; // Pressed/Released
        public event Action OnHideToggle;
        public event Action OnLogToggle;
        public event Action OnMenuToggle;

        public async UniTask InitializeAsync()
        {
            // Setup Actions via code to be self-contained
            SubmitAction = new InputAction("Submit", binding: "<Keyboard>/enter");
            SubmitAction.AddBinding("<Keyboard>/space");
            SubmitAction.AddBinding("<Gamepad>/buttonSouth"); // A/Cross
            SubmitAction.AddBinding("<Mouse>/leftButton");

            CancelAction = new InputAction("Cancel", binding: "<Keyboard>/escape");
            CancelAction.AddBinding("<Gamepad>/buttonEast"); // B/Circle

            SkipAction = new InputAction("Skip", binding: "<Keyboard>/leftCtrl");
            SkipAction.AddBinding("<Gamepad>/rightShoulder");

            HideAction = new InputAction("Hide", binding: "<Keyboard>/h");
            HideAction.AddBinding("<Gamepad>/buttonNorth"); // Y/Triangle

            LogAction = new InputAction("Log", binding: "<Keyboard>/l");
            LogAction.AddBinding("<Gamepad>/dpad/up");
            
            MenuAction = new InputAction("Menu", binding: "<Keyboard>/m");
            MenuAction.AddBinding("<Gamepad>/start");

            NavigateAction = new InputAction("Navigate", binding: "<Gamepad>/dpad");
            NavigateAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

            // Subscribe
            SubmitAction.performed += ctx => OnSubmit?.Invoke();
            CancelAction.performed += ctx => OnCancel?.Invoke();
            
            SkipAction.started += ctx => OnSkip?.Invoke(true);
            SkipAction.canceled += ctx => OnSkip?.Invoke(false);
            
            HideAction.performed += ctx => OnHideToggle?.Invoke();
            LogAction.performed += ctx => OnLogToggle?.Invoke();
            MenuAction.performed += ctx => OnMenuToggle?.Invoke();

            EnableInput();

            Debug.Log("[InputService] Initialized.");
            await UniTask.CompletedTask;
        }

        public void EnableInput()
        {
            SubmitAction?.Enable();
            CancelAction?.Enable();
            SkipAction?.Enable();
            HideAction?.Enable();
            LogAction?.Enable();
            MenuAction?.Enable();
            NavigateAction?.Enable();
        }

        public void DisableInput()
        {
            SubmitAction?.Disable();
            CancelAction?.Disable();
            SkipAction?.Disable();
            HideAction?.Disable();
            LogAction?.Disable();
            MenuAction?.Disable();
            NavigateAction?.Disable();
        }

        // Helper for Polling if needed
        public bool IsSkipPressed => SkipAction != null && SkipAction.IsPressed();
        public bool IsSubmitPressed => SubmitAction != null && SubmitAction.WasPressedThisFrame(); // Note: WasPressedThisFrame relies on Update.
        
        // Note: New Input System 'WasPressedThisFrame' behaves differently than legacy. 
        // Event-based approach is safer for 'OnSubmit'. 
        // For polling Skip in a loop, IsPressed() is good.

        private void OnDestroy()
        {
            DisableInput();
            SubmitAction?.Dispose();
            CancelAction?.Dispose();
            SkipAction?.Dispose();
            HideAction?.Dispose();
            LogAction?.Dispose();
            MenuAction?.Dispose();
            NavigateAction?.Dispose();
        }

        public void ResetService()
        {
            // Reset state if needed
        }
    }
}

