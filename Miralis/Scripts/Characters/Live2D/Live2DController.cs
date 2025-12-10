using UnityEngine;
using VSNL.Core;
using VSNL.Services;

namespace VSNL.Characters.Live2D
{
    // Mocking Cubism namespaces to compile without the SDK
    // In real project, replace with actual using Live2D.Cubism...
    
    public class Live2DController : MonoBehaviour
    {
        [Header("Parameters")]
        public string MouthOpenParam = "ParamMouthOpen";
        public float LipSyncSensivity = 2.0f;

        private Animator _animator;
        // private CubismModel _model; // Real SDK

        private float[] _audioSamples;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            // _model = GetComponent<CubismModel>();
            _audioSamples = new float[256];
        }

        private void Update()
        {
            UpdateLipSync();
        }

        private void UpdateLipSync()
        {
            var audioMgr = Engine.Instance.GetService<AudioManager>();
            if (audioMgr != null && audioMgr.VoiceSource != null && audioMgr.VoiceSource.isPlaying)
            {
                // Simple RMS
                audioMgr.VoiceSource.GetOutputData(_audioSamples, 0);
                
                float sum = 0;
                foreach (var s in _audioSamples) sum += s * s;
                float rms = Mathf.Sqrt(sum / _audioSamples.Length);
                
                float value = Mathf.Clamp01(rms * LipSyncSensivity);
                
                // In generic Live2D, we set parameter.
                // Since we don't have SDK installed, we mock this call.
                // SetParameterValue(MouthOpenParam, value);
            }
            else
            {
                 // SetParameterValue(MouthOpenParam, 0);
            }
        }

        public void PlayMotion(string motionName)
        {
            if (_animator)
            {
                _animator.Play(motionName);
            }
        }

        public void SetExpression(string expressionName)
        {
            // Usually triggers a CubismExpressionController or sets params directly.
            // For now, we assume Animator states or mocking log.
            // Debug.Log($"[Live2D] Set Expression: {expressionName}");
            if (_animator)
            {
                // Option A: Trigger in animator
                // _animator.SetTrigger(expressionName);
                
                // Option B: Generic implementation
            }
        }

        /*
        private void SetParameterValue(string id, float value)
        {
             // Actual SDK call
             // var param = _model.Parameters.FindById(id);
             // if (param) param.Value = value;
        }
        */
    }
}
