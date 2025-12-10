using System.Text.RegularExpressions;

namespace VSNL.Core.Utils
{
    public static class TextFormatter
    {
        // Patterns
        // 1. Furigana: {Kanji|Reading} -> <ruby="Reading">Kanji</ruby>
        // Note: Using non-greedy matching (?) to handle multiple per line
        private const string REGEX_FURIGANA = @"\{([^|]+)\|([^}]+)\}";
        
        // 2. Bold: **Text** -> <b>Text</b>
        private const string REGEX_BOLD = @"\*\*([^*]+)\*\*";
        
        // 3. Italic: *Text* -> <i>Text</i>
        private const string REGEX_ITALIC = @"\*([^*]+)\*";

        /// <summary>
        /// Formats VSNL raw text into Unity Rich Text.
        /// </summary>
        public static string FormatText(string rawText)
        {
            if (string.IsNullOrEmpty(rawText)) return rawText;

            string processed = rawText;

            // Apply Furigana first (brackets might contain other symbols, but usually don't overlap bold/italic boundaries in simple parser)
            processed = Regex.Replace(processed, REGEX_FURIGANA, "<ruby=\"$2\">$1</ruby>");
            
            // Apply Bold (Stronger delimiter ** checked before *)
            processed = Regex.Replace(processed, REGEX_BOLD, "<b>$1</b>");
            
            // Apply Italic
            processed = Regex.Replace(processed, REGEX_ITALIC, "<i>$1</i>");

            return processed;
        }
    }
}
