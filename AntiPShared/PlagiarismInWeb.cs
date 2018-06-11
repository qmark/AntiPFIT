using System.Collections.Generic;

namespace AntiPShared
{
    public class PlagiarismInWeb
    {
        public string[] Words { get; set; }
        public int WordCount { get; set; }
        public Dictionary<string, List<int>> OrderedUrlToWordsIndexes { get; set; }
        public List<string> OrderedUrls { get; set; }
      
    }
}