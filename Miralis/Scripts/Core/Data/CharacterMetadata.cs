using UnityEngine;
using System.Collections.Generic;
using System;

namespace VSNL.Core.Data
{
    [CreateAssetMenu(fileName = "CharacterMetadata", menuName = "VSNL/Character Metadata")]
    public class CharacterMetadata : ScriptableObject
    {
        [Serializable]
        public struct EmotionData
        {
            public string EmotionName;
            public Sprite SpriteAsset;
            public string Live2DExpressionParam; // e.g. "Happy"
        }

        [Serializable]
        public class CharacterData
        {
            public string CharacterID; // "Elysia"
            public string DisplayName; // "Elysia" (for Namebox)
            public Color NameColor = Color.white;
            public GameObject Live2DModelPrefab; // New field
            public List<EmotionData> Emotions = new List<EmotionData>();

            public Sprite GetSprite(string emotion)
            {
                foreach (var e in Emotions)
                {
                    if (e.EmotionName.Equals(emotion, StringComparison.OrdinalIgnoreCase))
                    {
                        return e.SpriteAsset;
                    }
                }
                return null;
            }
        }

        public List<CharacterData> Characters = new List<CharacterData>();

        public CharacterData GetCharacter(string id)
        {
            foreach (var c in Characters)
            {
                if (c.CharacterID.Equals(id, StringComparison.OrdinalIgnoreCase)) return c;
            }
            return null;
        }
    }
}
