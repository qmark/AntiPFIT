using System.Collections.Generic;

namespace AntiPShared
{
    public class CheckResult
    {
        public string Url { get; set; }
        public List<string> CommonTextParts { get; set; }
        public double CharactersPercentage { get; set; }
    }
}
