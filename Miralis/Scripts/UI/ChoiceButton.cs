using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace VSNL.UI
{
    public class ChoiceButton : MonoBehaviour
    {
        public Button ButtonComponent;
        public TextMeshProUGUI TextComponent;

        private Action<string> _onClicked;
        private string _targetLabel;

        public void Setup(string text, string targetLabel, Action<string> onClicked)
        {
            if (TextComponent) TextComponent.text = text;
            _targetLabel = targetLabel;
            _onClicked = onClicked;

            if (ButtonComponent)
            {
                ButtonComponent.onClick.RemoveAllListeners();
                ButtonComponent.onClick.AddListener(HandleClick);
            }
        }

        private void HandleClick()
        {
            _onClicked?.Invoke(_targetLabel);
        }
    }
}
