using System;
using System.Collections.Generic;
using System.Text;

namespace AntiPShared
{
    public class PlagiarismInLocalDB
    {
        public string[] Words { get; set; }
        public int WordCount { get; set; }
        public Dictionary<int, HashSet<(int DBDocIndex, int initialDocIndex)>> PlagiarismResult  { get; set; }
        public double Vodnost { get; set; }
        public double Toshnotnost { get; set; }
    }
}
