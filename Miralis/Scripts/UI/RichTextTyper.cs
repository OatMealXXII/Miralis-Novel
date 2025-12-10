using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem;

namespace VSNL.UI
{
    public class RichTextTyper : MonoBehaviour
    {
        [Header("Settings")]
        public float DefaultTypeSpeed = 0.05f;
        public float ShakeAmount = 5.0f;
        public float WaveAmount = 5.0f;
        public float WaveSpeed = 5.0f;

        private TextMeshProUGUI _tmp;
        private string _rawText;
        private string _cleanText;
        
        private struct TyperCommand
        {
            public int Index;
            public string Type; // "wait", "speed"
            public float Value;
        }

        private struct TextEffect
        {
            public int StartIndex;
            public int EndIndex;
            public string Type; // "shake", "wave"
        }

        private List<TyperCommand> _commands = new List<TyperCommand>();
        private List<TextEffect> _effects = new List<TextEffect>();
        
#pragma warning disable 0414
        private bool _isTyping;
#pragma warning restore 0414
        private bool _cancelTyping;
        private float _currentSpeed;

        private void Awake()
        {
            _tmp = GetComponent<TextMeshProUGUI>();
        }

        public async UniTask TypeAsync(string rawText, System.Func<bool> isSkip)
        {
            _rawText = rawText;
            ParseText(rawText);
            
            _tmp.text = _cleanText;
            _tmp.maxVisibleCharacters = 0;
            _isTyping = true;
            _cancelTyping = false;
            _currentSpeed = DefaultTypeSpeed;

            int totalChars = _cleanText.Length;
            int currentVisible = 0;

            // Typewriter Loop
            while (currentVisible < totalChars)
            {
                if (isSkip() || _cancelTyping)
                {
                    currentVisible = totalChars;
                    _tmp.maxVisibleCharacters = currentVisible;
                    break;
                }

                // Check for commands at this index
                if (_commands.Count > 0)
                {
                    await ProcessCommandsAsync(currentVisible);
                }

                currentVisible++;
                _tmp.maxVisibleCharacters = currentVisible;

                // Speed Check (if slow)
                if (_currentSpeed > 0)
                {
                    await UniTask.Delay(System.TimeSpan.FromSeconds(_currentSpeed));
                }
                
                // Input fast-forward logic (click to complete line)
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    _cancelTyping = true;
                }
            }
            
            _isTyping = false;
            _tmp.maxVisibleCharacters = totalChars; // Ensure full show
        }

        private void Update()
        {
            if (_tmp == null || _effects.Count == 0) return;

            // Vertex Animation
            _tmp.ForceMeshUpdate();
            var textInfo = _tmp.textInfo;

            bool hasChanges = false;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                // Check if this char is in any effect range
                foreach (var effect in _effects)
                {
                    if (i >= effect.StartIndex && i < effect.EndIndex)
                    {
                        var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                        int vertexIndex = charInfo.vertexIndex;

                        Vector3 offset = Vector3.zero;

                        if (effect.Type == "shake")
                        {
                            offset = new Vector3(Random.Range(-ShakeAmount, ShakeAmount), Random.Range(-ShakeAmount, ShakeAmount), 0);
                        }
                        else if (effect.Type == "wave")
                        {
                            offset = new Vector3(0, Mathf.Sin(Time.time * WaveSpeed + i) * WaveAmount, 0);
                        }

                        // Apply offset to all 4 vertices of the character
                        verts[vertexIndex + 0] += offset;
                        verts[vertexIndex + 1] += offset;
                        verts[vertexIndex + 2] += offset;
                        verts[vertexIndex + 3] += offset;

                        hasChanges = true;
                    }
                }
            }

            if (hasChanges)
            {
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    _tmp.UpdateGeometry(meshInfo.mesh, i);
                }
            }
        }

        private void ParseText(string text)
        {
            _commands.Clear();
            _effects.Clear();
            _cleanText = "";

            // Regex for tags: <tag:param> or <tag>...</tag>
            // We'll iterate manually to be robust or use simple Regex replacement strategy
            // A simple stack-based parser is better for nested tags, but let's stick to Regex for simplicity of this task.
            // Assumption: Tags are well-formed.

            // 1. Process custom tags placeholders to map indices
            // This is complex because removing tags changes indices.
            // Approach: Iterate char by char, building clean string.
            
            int cleanIndex = 0;
            // Stack for effect start indices: Key=Type, Value=StartIndex
            // Supporting simple non-nested same-type tags for now.
            Dictionary<string, int> effectStarts = new Dictionary<string, int>();

            string pattern = @"<(/?)(\w+)(?::([\d\.]+))?>"; // Matches <tag>, </tag>, <tag:value>
            
            int lastPos = 0;
            foreach (Match m in Regex.Matches(text, pattern))
            {
                // Append text before tag
                string segment = text.Substring(lastPos, m.Index - lastPos);
                _cleanText += segment;
                cleanIndex += segment.Length;

                lastPos = m.Index + m.Length;

                bool isClose = m.Groups[1].Value == "/";
                string tag = m.Groups[2].Value.ToLower();
                string param = m.Groups[3].Value;

                if (tag == "color" || tag == "b" || tag == "i" || tag == "size")
                {
                    // These are TMP tags, keep them in cleanText? 
                    // If we keep them, they don't affect typing index usually? 
                    // Actually TMP tags DO complicate the "visible character" count vs "string length".
                    // But maxVisibleCharacters handles it gracefully. 
                    // HOWEVER, our cleanIndex tracking needs to know if we keep them.
                    // Let's KEEP standard TMP tags in the text, so they work.
                    // But Wait/Speed/Shake/Wave must be stripped.
                    
                    _cleanText += m.Value; // Add the tag itself
                    // Note: TMP tags are part of the string but distinct from visible characters.
                    // Managing indices for Shake/Wave relative to TMP tags is tricky.
                    // We will assume Effects are applied to *Visible Character Indices*.
                    // TextInfo works on visible chars mostly.
                    continue; 
                }

                if (isClose)
                {
                    // End Effect
                    if (effectStarts.ContainsKey(tag))
                    {
                        _effects.Add(new TextEffect 
                        { 
                            StartIndex = effectStarts[tag], 
                            EndIndex = cleanIndex, // Current visible index
                            Type = tag 
                        });
                        effectStarts.Remove(tag);
                    }
                }
                else
                {
                    // Start Tag or Command
                    if (tag == "wait")
                    {
                        float val = 0.5f;
                        float.TryParse(param, out val);
                        _commands.Add(new TyperCommand { Index = cleanIndex, Type = "wait", Value = val });
                    }
                    else if (tag == "speed")
                    {
                        float val = DefaultTypeSpeed;
                        float.TryParse(param, out val);
                        _commands.Add(new TyperCommand { Index = cleanIndex, Type = "speed", Value = val });
                    }
                    else if (tag == "shake" || tag == "wave")
                    {
                        effectStarts[tag] = cleanIndex;
                    }
                }
            }
            // Append remaining text
            _cleanText += text.Substring(lastPos);
        }

        private async UniTask ProcessCommandsAsync(int index)
        {
             foreach (var cmd in _commands)
            {
                if (cmd.Index == index)
                {
                     if (cmd.Type == "wait")
                    {
                        await UniTask.Delay(System.TimeSpan.FromSeconds(cmd.Value));
                    }
                     else if (cmd.Type == "speed")
                    {
                        _currentSpeed = cmd.Value;
                    }
                }
            }
        }
    }
}
