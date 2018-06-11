using System.Collections.Generic;

namespace AntiPShared
{
    public class Plagiarism<TSourceId>
    {
        public Dictionary<TSourceId, HashSet<int>> SourceIdToSourceWordsIndexes { get; set; } = new Dictionary<TSourceId, HashSet<int>>();
        public Dictionary<TSourceId, HashSet<int>> SourceIdToInitialWordsIndexes { get; set; } = new Dictionary<TSourceId, HashSet<int>>();
        public Dictionary<int, HashSet<TSourceId>> InitialWordIndexToSourceIds { get; set; } = new Dictionary<int, HashSet<TSourceId>>();
        public Dictionary<TSourceId, string> SourceIdToInitialDocumentHtml { get; set; } = new Dictionary<TSourceId, string>();

        public void Add(Plagiarism<TSourceId> plagiarism)
        {
            foreach (var kvp in plagiarism.SourceIdToSourceWordsIndexes)
            {
                if (SourceIdToSourceWordsIndexes.TryGetValue(kvp.Key, out HashSet<int> plagiarizedDBWordsIndexes))
                {
                    plagiarizedDBWordsIndexes.UnionWith(kvp.Value);
                    SourceIdToInitialWordsIndexes[kvp.Key].UnionWith(plagiarism.SourceIdToInitialWordsIndexes[kvp.Key]);
                }
                else
                {
                    SourceIdToSourceWordsIndexes.Add(kvp.Key, new HashSet<int>(kvp.Value));
                    SourceIdToInitialWordsIndexes.Add(kvp.Key, new HashSet<int>(plagiarism.SourceIdToInitialWordsIndexes[kvp.Key]));
                }
            }
            foreach (var kvp in plagiarism.InitialWordIndexToSourceIds)
            {
                if (InitialWordIndexToSourceIds.TryGetValue(kvp.Key, out HashSet<TSourceId> sourceDocumentIds))
                {
                    sourceDocumentIds.UnionWith(kvp.Value);
                }
                else
                {
                    InitialWordIndexToSourceIds.Add(kvp.Key, new HashSet<TSourceId>(kvp.Value));
                }
            }
        }
    }
}