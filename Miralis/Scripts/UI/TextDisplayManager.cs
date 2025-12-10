using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using VSNL.Core;

namespace VSNL.UI
{
    public class TextDisplayManager : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI SpeakerText;
        public TextMeshProUGUI DialogueText;
        public GameObject SpeakerNamePanel; // Optional: To hide if empty

        [Header("Configuration")]
        public float BaseTypewriterSpeed = 0.05f;
        public float FastTypewriterSpeed = 0.01f;
        public float AutoPlayBaseDelay = 1.0f;
        public float AutoPlayCharDelay = 0.05f;

        private bool _isTyping;
        private bool _cancelTyping;

        public async UniTask ShowTextAsync(string speaker, string text, Func<bool> isAuto, Func<bool> isSkip)
        {
            // 1. Setup Speaker
            if (SpeakerText)
            {
                if (string.IsNullOrEmpty(speaker) || speaker.Equals("Narrator", StringComparison.OrdinalIgnoreCase))
                {
                    SpeakerText.text = "";
                    if (SpeakerNamePanel) SpeakerNamePanel.SetActive(false);
                }
                else
                {
                    SpeakerText.text = speaker;
                    if (SpeakerNamePanel) SpeakerNamePanel.SetActive(true);
                }
            }

            // 2. Clear Text & Setup Typer
            if (DialogueText)
            {
                DialogueText.text = "";
                var typer = DialogueText.GetComponent<RichTextTyper>();
                if (typer == null) typer = DialogueText.gameObject.AddComponent<RichTextTyper>();
                
                // Configure Typer speed defaults if needed
                typer.DefaultTypeSpeed = isSkip() ? 0 : BaseTypewriterSpeed;

                // 3. Type
                await typer.TypeAsync(text, isSkip);
            }

            // 4. Wait for Advance
            // Typer finishes when text is full. Now we wait for user click/auto.
            
            float autoTimer = 0f;
            float autoDuration = AutoPlayBaseDelay + (text.Length * AutoPlayCharDelay);

            var input = Engine.Instance.GetService<InputService>();

            while (true)
            {
                // User Input
                if (input != null && input.IsSubmitPressed)
                {
                    break;
                }

                if (isAuto())
                {
                    autoTimer += Time.deltaTime;
                    if (autoTimer >= autoDuration) break;
                }
                
                if (isSkip() || (input != null && input.IsSkipPressed))
                {
                    await UniTask.Delay(50);
                    break;
                }

                await UniTask.Yield();
            }
            
            await UniTask.DelayFrame(1); 
        }
    }
}
