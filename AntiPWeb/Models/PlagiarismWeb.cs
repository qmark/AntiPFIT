using System.Collections.Generic;

namespace AntiPShared
{
    public class PlagiarismWeb
    {
        public Dictionary<string, HashSet<int>> UrlToWebPageWordsIndexes { get; set; } = new Dictionary<string, HashSet<int>>();
        public Dictionary<string, HashSet<int>> UrlToInitialWordsIndexes { get; set; } = new Dictionary<string, HashSet<int>>();
        public Dictionary<int, HashSet<string>> InitialWordIndexToUrls { get; set; } = new Dictionary<int, HashSet<string>>();
        public Dictionary<string, string> UrlToInitialDocumentHtml { get; set; } = new Dictionary<string, string>();
    }
}