using System;
using System.Collections.Generic;

namespace AntiPShared
{
    public class Plagiarism<TSourceId>
    {
        public Dictionary<TSourceId, HashSet<int>> SourceIdToSourceWordsIndexes { get; set; } = new Dictionary<TSourceId, HashSet<int>>();
        public Dictionary<TSourceId, HashSet<int>> SourceIdToInitialWordsIndexes { get; set; } = new Dictionary<TSourceId, HashSet<int>>();
        public Dictionary<int, HashSet<TSourceId>> InitialWordIndexToSourceIds { get; set; } = new Dictionary<int, HashSet<TSourceId>>();
        public Dictionary<TSourceId, string> SourceIdToInitialDocumentHtml { get; set; } = new Dictionary<TSourceId, string>();
        public string DebugLogs { get; set; }
        public const int WordCap = 10;

        public void Add(Plagiarism<TSourceId> plagiarism)
        {
            foreach (var kvp in plagiarism.SourceIdToSourceWordsIndexes)
            {
                if (SourceIdToSourceWordsIndexes.TryGetValue(kvp.Key, out HashSet<int> plagiarizedSourceWordsIndexes))
                {
                    plagiarizedSourceWordsIndexes.UnionWith(kvp.Value);
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

        private const int MagicDistanceBetweenIndexes = 4;
        public static Plagiarism<TSourceId> FindPlagiarism(Dictionary<TSourceId, List<List<int>>> sourceIdToSourceWordsPositionsForShingle, List<int> initialDocIndexesForShingle)
        {
            var plagiarismForShingle = new Plagiarism<TSourceId>();

            foreach (var kvp in sourceIdToSourceWordsPositionsForShingle)
            {
                for (int firstWordPosition = 0; firstWordPosition < kvp.Value[0].Count; firstWordPosition++)
                {
                    var sourceWordsIndexes = new List<int>
                {
                    kvp.Value[0][firstWordPosition]
                };

                    var initialDocIndexes = new List<int>
                {
                    initialDocIndexesForShingle[0]
                };

                    bool hasEverything = true;
                    for (int i = 1; hasEverything && i < Shingle.Lenght; i++)
                    {
                        int wordIndexOnDistance = kvp.Value[i].Find(item => Math.Abs(item - sourceWordsIndexes[i - 1]) < MagicDistanceBetweenIndexes);

                        if (wordIndexOnDistance != 0)
                        {
                            sourceWordsIndexes.Add(wordIndexOnDistance);
                            initialDocIndexes.Add(initialDocIndexesForShingle[i]);
                        }
                        else
                        {
                            hasEverything = false;
                            break;
                        }
                    }

                    if (hasEverything)
                    {
                        if (plagiarismForShingle.SourceIdToSourceWordsIndexes.TryGetValue(kvp.Key, out HashSet<int> DBDocPlagiarizedPositions))
                        {
                            DBDocPlagiarizedPositions.UnionWith(sourceWordsIndexes);
                            plagiarismForShingle.SourceIdToInitialWordsIndexes[kvp.Key].UnionWith(initialDocIndexes);
                        }
                        else
                        {
                            plagiarismForShingle.SourceIdToSourceWordsIndexes.Add(kvp.Key, new HashSet<int>(sourceWordsIndexes));
                            plagiarismForShingle.SourceIdToInitialWordsIndexes.Add(kvp.Key, new HashSet<int>(initialDocIndexes));
                        }

                        if (plagiarismForShingle.InitialWordIndexToSourceIds.ContainsKey(initialDocIndexesForShingle[0]))
                        {
                            for (int i = 0; i < Shingle.Lenght; i++)
                            {
                                plagiarismForShingle.InitialWordIndexToSourceIds[initialDocIndexesForShingle[i]].Add(kvp.Key);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Shingle.Lenght; i++)
                            {
                                plagiarismForShingle.InitialWordIndexToSourceIds.Add(initialDocIndexesForShingle[i], new HashSet<TSourceId>() { kvp.Key });
                            }
                        }
                    }
                }
            }

            return plagiarismForShingle;
        }

    }
}