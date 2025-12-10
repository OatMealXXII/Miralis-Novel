using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;

namespace VSNL.Core
{
    public class SettingsManager : MonoBehaviour, IGameService
    {
        [Header("Audio")]
        public AudioMixer MainMixer; // Assign in Inspector
        public const string BGMParam = "BGMVol";
        public const string SFXParam = "SFXVol";
        public const string VoiceParam = "VoiceVol";

        // Properties with backing fields for runtime usage
        public float BGMVolume { get; private set; } = 1.0f;
        public float SFXVolume { get; private set; } = 1.0f;
        public float VoiceVolume { get; private set; } = 1.0f;

        [Header("Gameplay")]
        public float TextSpeed { get; private set; } = 0.05f; // Seconds per char
        public float AutoPlaySpeed { get; private set; } = 1.0f; // Multiplier or Base Delay

        public async UniTask InitializeAsync()
        {
            LoadSettings();
            Debug.Log("[SettingsManager] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            // Reset to defaults
            BGMVolume = 1.0f;
            SFXVolume = 1.0f;
            VoiceVolume = 1.0f;
            TextSpeed = 0.05f;
            AutoPlaySpeed = 1.0f;
            ApplyAudioSettings();
        }

        public void LoadSettings()
        {
            BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
            VoiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 1.0f);
            
            TextSpeed = PlayerPrefs.GetFloat("TextSpeed", 0.05f);
            AutoPlaySpeed = PlayerPrefs.GetFloat("AutoPlaySpeed", 1.5f);

            ApplyAudioSettings();
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("BGMVolume", BGMVolume);
            PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
            PlayerPrefs.SetFloat("VoiceVolume", VoiceVolume);
            PlayerPrefs.SetFloat("TextSpeed", TextSpeed);
            PlayerPrefs.SetFloat("AutoPlaySpeed", AutoPlaySpeed);
            PlayerPrefs.Save();
        }

        // Setters for UI
        public void SetBGMVolume(float v) { BGMVolume = v; SetMixerVolume(BGMParam, v); }
        public void SetSFXVolume(float v) { SFXVolume = v; SetMixerVolume(SFXParam, v); }
        public void SetVoiceVolume(float v) { VoiceVolume = v; SetMixerVolume(VoiceParam, v); }
        
        public void SetTextSpeed(float v) { TextSpeed = v; }
        public void SetAutoPlaySpeed(float v) { AutoPlaySpeed = v; }

        private void ApplyAudioSettings()
        {
            SetMixerVolume(BGMParam, BGMVolume);
            SetMixerVolume(SFXParam, SFXVolume);
            SetMixerVolume(VoiceParam, VoiceVolume);
        }

        private void SetMixerVolume(string param, float normalizedValue)
        {
            if (!MainMixer) return;

            // Convert 0-1 to dB (-80 to 0)
            // Warning: Log10(0) is -Infinity. Clamp minimum.
            float db = -80f;
            if (normalizedValue > 0.0001f)
            {
                db = Mathf.Log10(normalizedValue) * 20f;
            }
            
            MainMixer.SetFloat(param, db);
        }
    }
}

