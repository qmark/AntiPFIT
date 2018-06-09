using System;
using System.Linq;

namespace AntiPShared
{
    public class TextAnalyzer
    {
        public static (double vodnost, double toshnotnost) Analyze(string[] simplifiedWords)
        {
            double vodnost = 0;
            for (int i = 0; i < simplifiedWords.Length; i++)
            {
                if (TextManager.StopWords.Contains(simplifiedWords[i]))
                    vodnost++;
            }
            vodnost /= Convert.ToDouble(simplifiedWords.Length);

            double toshnotnost = (simplifiedWords.Length - simplifiedWords.Distinct().Count()) / Convert.ToDouble(simplifiedWords.Length);

            return (vodnost, toshnotnost);
        }
    }
}