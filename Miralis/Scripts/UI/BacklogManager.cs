using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VSNL.Core;

namespace VSNL.UI
{
    public struct LogEntry
    {
        public string Speaker;
        public string Text;
        public string VoiceClip;
    }

    public class BacklogManager : MonoBehaviour, IGameService
    {
        public List<LogEntry> Logs { get; private set; } = new List<LogEntry>();
        public int MaxLogCount = 100;

        public event Action OnLogAdded;

        public async UniTask InitializeAsync()
        {
            Logs.Clear();
            Debug.Log("[BacklogManager] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            Logs.Clear();
            OnLogAdded?.Invoke();
        }

        public void AddLog(string speaker, string text, string voiceClip = null)
        {
            var entry = new LogEntry
            {
                Speaker = speaker,
                Text = text,
                VoiceClip = voiceClip
            };

            Logs.Add(entry);

            if (Logs.Count > MaxLogCount)
            {
                Logs.RemoveAt(0);
            }

            OnLogAdded?.Invoke();
        }
    }
}

