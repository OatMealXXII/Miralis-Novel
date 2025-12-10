using UnityEngine;
using UnityEngine.UI;
using VSNL.Core;

namespace VSNL.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("Audio Sliders")]
        public Slider BGMSlider;
        public Slider SFXSlider;
        public Slider VoiceSlider;

        [Header("Gameplay Sliders")]
        public Slider TextSpeedSlider;
        public Slider AutoSpeedSlider;

        public GameObject Panel;

        private SettingsManager _settings;

        private void Start()
        {
            _settings = Engine.Instance.GetService<SettingsManager>();
            // Init sliders if settings ready
            if (_settings != null) RefreshUI();
        }

        public void Toggle()
        {
            if (Panel)
            {
                bool isActive = !Panel.activeSelf;
                Panel.SetActive(isActive);
                if (isActive) RefreshUI();
                else _settings?.SaveSettings();
            }
        }

        public void RefreshUI()
        {
            if (_settings == null) return;

            if (BGMSlider) { BGMSlider.value = _settings.BGMVolume; BGMSlider.onValueChanged.AddListener(OnBGMChanged); }
            if (SFXSlider) { SFXSlider.value = _settings.SFXVolume; SFXSlider.onValueChanged.AddListener(OnSFXChanged); }
            if (VoiceSlider) { VoiceSlider.value = _settings.VoiceVolume; VoiceSlider.onValueChanged.AddListener(OnVoiceChanged); }
            
            if (TextSpeedSlider) { TextSpeedSlider.value = _settings.TextSpeed; TextSpeedSlider.onValueChanged.AddListener(OnTextSpeedChanged); }
            if (AutoSpeedSlider) { AutoSpeedSlider.value = _settings.AutoPlaySpeed; AutoSpeedSlider.onValueChanged.AddListener(OnAutoSpeedChanged); }
        }

        private void OnBGMChanged(float v) => _settings?.SetBGMVolume(v);
        private void OnSFXChanged(float v) => _settings?.SetSFXVolume(v);
        private void OnVoiceChanged(float v) => _settings?.SetVoiceVolume(v);
        private void OnTextSpeedChanged(float v) => _settings?.SetTextSpeed(v); // Note: UI might need to invert logic if slider is "Speed" vs "Delay"
        private void OnAutoSpeedChanged(float v) => _settings?.SetAutoPlaySpeed(v);
    }
}
