using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VSNL.UI
{
    public class BacklogEntryUI : MonoBehaviour
    {
        public TextMeshProUGUI SpeakerText;
        public TextMeshProUGUI DialogueText;
        public Button ReplayVoiceButton;

        private string _voiceClipName;

        public void Setup(string speaker, string text, string voiceClip)
        {
            if (SpeakerText) SpeakerText.text = speaker;
            if (DialogueText) DialogueText.text = text;

            _voiceClipName = voiceClip;
            if (ReplayVoiceButton)
            {
                if (!string.IsNullOrEmpty(voiceClip))
                {
                    ReplayVoiceButton.gameObject.SetActive(true);
                    ReplayVoiceButton.onClick.RemoveAllListeners();
                    ReplayVoiceButton.onClick.AddListener(OnReplayClicked);
                }
                else
                {
                    ReplayVoiceButton.gameObject.SetActive(false);
                }
            }
        }

        private void OnReplayClicked()
        {
            // Replay logic via AudioManager
            // We need access to AudioManager. Ideally Engine.Instance.GetService<AudioManager>().PlayVoice(...)
            // For now, let's just Log or Stub.
            // If AudioManager had PlaySFX, we can use that.
            var audio = Core.Engine.Instance.GetService<Services.AudioManager>();
            if (audio != null && !string.IsNullOrEmpty(_voiceClipName))
            {
                audio.PlaySFX(_voiceClipName); // Reuse SFX for voice for now
            }
        }
    }
}
