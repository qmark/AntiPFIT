using System.Collections.Generic;

namespace AntiPShared
{
    public class PlagiarismInLocalDB
    {
        public string[] Words { get; set; }
        public int WordCount { get; set; }
        public Dictionary<int, HashSet<(int DBDocIndex, int initialDocIndex)>> PlagiarismResult  { get; set; }
        public double Vodnost { get; set; }
        public double Toshnotnost { get; set; }
        public string Text { get; set; }
    }
}
