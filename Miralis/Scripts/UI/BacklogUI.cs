using UnityEngine;
using VSNL.Core;
using System.Collections.Generic;

namespace VSNL.UI
{
    public class BacklogUI : MonoBehaviour
    {
        public GameObject BacklogPanel;
        public Transform ContentContainer;
        public GameObject EntryPrefab;

        private BacklogManager _manager;
        private List<GameObject> _spawnedEntries = new List<GameObject>();

        private void Start()
        {
            // Optionally auto-find manager or wait for init
            // Usually UI needs to wait for Engine Init. 
            // We can check in Update or rely on GameLoop.
        }

        public void Show()
        {
            if (BacklogPanel) BacklogPanel.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            if (BacklogPanel) BacklogPanel.SetActive(false);
        }

        public void Toggle()
        {
            if (BacklogPanel)
            {
                if (BacklogPanel.activeSelf) Hide();
                else Show();
            }
        }

        private void Refresh()
        {
            if (_manager == null)
            {
                _manager = Engine.Instance.GetService<BacklogManager>();
            }

            if (_manager == null) return;

            // Clear old
            foreach (var go in _spawnedEntries) Destroy(go);
            _spawnedEntries.Clear();

            // Spawn new
            if (EntryPrefab && ContentContainer)
            {
                foreach (var log in _manager.Logs)
                {
                    var obj = Instantiate(EntryPrefab, ContentContainer);
                    var entryUI = obj.GetComponent<BacklogEntryUI>();
                    if (entryUI)
                    {
                        entryUI.Setup(log.Speaker, log.Text, log.VoiceClip);
                    }
                    _spawnedEntries.Add(obj);
                }
            }
        }
    }
}
