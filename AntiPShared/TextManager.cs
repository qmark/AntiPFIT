using System.Collections.Generic;
using System.Linq;

namespace AntiPShared
{
    public class TextManager
    {
        public static List<string> WordsFromText(string text)
        {
            var words = text.Split().ToList();
            words.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            return words;
        }

        public static string SimplifyText(string text)
        {
            return new string(text.Replace('-', ' ').Where(c => !char.IsPunctuation(c)).ToArray()).ToLower();
        }
    }
}
