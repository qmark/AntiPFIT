using System.Collections.Generic;

namespace AntiPShared
{
    public class PlagiarismDB
    {
        public Dictionary<int, HashSet<int>> DocumentIdToDBWordsIndexes { get; set; } = new Dictionary<int, HashSet<int>>();
        public Dictionary<int, HashSet<int>> DocumentIdToInitialWordsIndexes { get; set; } = new Dictionary<int, HashSet<int>>();
        public Dictionary<int, HashSet<int>> InitialWordIndexToDocumentIds { get; set; } = new Dictionary<int, HashSet<int>>();
        public Dictionary<int, string> DocumentIdToInitialDocumentHtml { get; set; } = new Dictionary<int, string>();
    }
}