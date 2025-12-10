using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VSNL.Core;

namespace VSNL.Services
{
    public class AudioManager : MonoBehaviour, IGameService
    {
        private AudioSource _bgmSource1; // Primary
        private AudioSource _bgmSource2; // Secondary (for crossfade)
        private bool _isUsingSource1 = true;
        
        private AudioSource _sfxSource;
        private AudioSource _voiceSource;
        public AudioSource VoiceSource => _voiceSource; // Exposed for LipSync

        public float DefaultFadeDuration = 1.0f;

        public async UniTask InitializeAsync()
        {
            // BGM Sources
            _bgmSource1 = CreateSource("BGM_1", true);
            _bgmSource2 = CreateSource("BGM_2", true);
            _bgmSource2.volume = 0f; // Start silent

            // SFX / Voice
            _sfxSource = CreateSource("SFX", false);
            _voiceSource = CreateSource("Voice", false);

            Debug.Log("[AudioManager] Initialized.");
            await UniTask.CompletedTask;
        }

        private AudioSource CreateSource(string name, bool loop)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.loop = loop;
            src.playOnAwake = false;
            return src;
        }

        public void ResetService()
        {
            StopBGM();
            _sfxSource.Stop();
            _voiceSource.Stop();
        }

        private async UniTask<AudioClip> LoadClipAsync(string name, string subfolder)
        {
             var resources = Engine.Instance.GetService<ResourceManager>();
             if (resources == null) return null;

             var clip = await resources.LoadAssetAsync<AudioClip>($"Audio/{subfolder}/{name}");
             if (!clip) clip = await resources.LoadAssetAsync<AudioClip>($"Audio/{name}");
             
             if (!clip) Debug.LogWarning($"[AudioManager] Clip {name} not found in {subfolder}.");
             return clip;
        }

        // Updated callers to be async
        public async void PlayBGM(string clipName, float fadeDuration = -1)
        {
            if (fadeDuration < 0) fadeDuration = DefaultFadeDuration;

            var clip = await LoadClipAsync(clipName, "BGM");
            if (!clip) return;
            
            // ... same logic ...
            var active = _isUsingSource1 ? _bgmSource1 : _bgmSource2;
            var next = _isUsingSource1 ? _bgmSource2 : _bgmSource1;

            if (active.clip == clip && active.isPlaying) return; 

            CrossfadeAsync(active, next, clip, fadeDuration).Forget();
            _isUsingSource1 = !_isUsingSource1; 
        }

        public void StopBGM()
        {
            _bgmSource1.Stop();
            _bgmSource2.Stop();
        }

        private async UniTask CrossfadeAsync(AudioSource active, AudioSource next, AudioClip newClip, float duration)
        {
            // Next starts at 0 vol
            next.clip = newClip;
            next.volume = 0f;
            next.Play();

            // Active fades out, Next fades in
            var tasks = new System.Collections.Generic.List<UniTask>();
            
            tasks.Add(active.DOFade(0f, duration).AsyncWaitForCompletion().AsUniTask());
            tasks.Add(next.DOFade(1f, duration).AsyncWaitForCompletion().AsUniTask()); // Assuming 1f is max volume

            await UniTask.WhenAll(tasks);
            
            active.Stop();
        }

        public async void PlaySFX(string clipName)
        {
            var clip = await LoadClipAsync(clipName, "SFX");
            if (clip) _sfxSource.PlayOneShot(clip);
        }

        public async void PlayVoice(string clipName)
        {
            _voiceSource.Stop();
            var clip = await LoadClipAsync(clipName, "Voice");
            if (clip)
            {
                _voiceSource.clip = clip;
                _voiceSource.Play();
            }
        }
    }

}
