using DiffPlex.Model;
using System;
using System.Text;

namespace AntiPShared
{
    public static class UnidiffFormater
    {
        private const string InsertSymbolStart = "<em>";
        private const string InsertSymbolEnd = "</em>";
        private const string DeleteSymbolStart = "<font color='red'><strike>";
        private const string DeleteSymbolEnd = "</strike></font>";

        public static string Generate(DiffResult diffResult)
        {
            var uniLines = new StringBuilder();
            int blockBPosition = 0;

            foreach (var diffBlock in diffResult.DiffBlocks)
            {
                for (; blockBPosition < diffBlock.InsertStartB; blockBPosition++)
                    uniLines.Append(diffResult.PiecesNew[blockBPosition]);

                int i = 0;
                for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
                {
                    uniLines.Append(DeleteSymbolStart + diffResult.PiecesOld[i + diffBlock.DeleteStartA] + DeleteSymbolEnd);
                    uniLines.Append(InsertSymbolStart + diffResult.PiecesNew[i + diffBlock.InsertStartB] + InsertSymbolEnd);
                    blockBPosition++;
                }

                if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
                {
                    uniLines.Append(DeleteSymbolStart);

                    for (; i < diffBlock.DeleteCountA; i++)
                        uniLines.Append(diffResult.PiecesOld[i + diffBlock.DeleteStartA]);

                    uniLines.Append(DeleteSymbolEnd);
                }
                else
                {
                    if (i < diffBlock.InsertCountB)
                    {
                        uniLines.Append(InsertSymbolStart);

                        for (; i < diffBlock.InsertCountB; i++)
                        {
                            uniLines.Append(diffResult.PiecesNew[i + diffBlock.InsertStartB]);
                            blockBPosition++;
                        }

                        uniLines.Append(InsertSymbolEnd);
                    }
                }
            }

            for (; blockBPosition < diffResult.PiecesNew.Length; blockBPosition++)
                uniLines.Append(diffResult.PiecesNew[blockBPosition]);

            return uniLines.ToString();
        }

        public static string CommonPart(DiffResult diffResult)
        {
            var uniLines = new StringBuilder();
            int blockBPosition = 0;

            foreach (var diffBlock in diffResult.DiffBlocks)
            {
                for (; blockBPosition < diffBlock.InsertStartB; blockBPosition++)
                    uniLines.Append(diffResult.PiecesNew[blockBPosition]);

                int i = 0;
                var min = Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB);
                if (i < min)
                {
                    i = min;
                    blockBPosition += min;
                }
                //for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
                //{
                //    //uniLines.Append(DeleteSymbolStart + diffResult.PiecesOld[i + diffBlock.DeleteStartA] + DeleteSymbolEnd);
                //    //uniLines.Append(InsertSymbolStart + diffResult.PiecesNew[i + diffBlock.InsertStartB] + InsertSymbolEnd);
                //    blockBPosition++;
                //}

                if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
                {
                    //uniLines.Append(DeleteSymbolStart);

                    i = diffBlock.DeleteCountA;
                    //for (; i < diffBlock.DeleteCountA; i++) ;
                    //uniLines.Append(diffResult.PiecesOld[i + diffBlock.DeleteStartA]);


                    //uniLines.Append(DeleteSymbolEnd);
                }
                else
                {
                    if (i < diffBlock.InsertCountB)
                    {
                        //uniLines.Append(InsertSymbolStart);

                        blockBPosition += diffBlock.InsertCountB - i;
                        i = diffBlock.InsertCountB;
                        //for (; i < diffBlock.InsertCountB; i++)
                        //{
                        //    //uniLines.Append(diffResult.PiecesNew[i + diffBlock.InsertStartB]);
                        //    blockBPosition++;
                        //}

                        //uniLines.Append(InsertSymbolEnd);
                    }
                }
            }

            for (; blockBPosition < diffResult.PiecesNew.Length; blockBPosition++)
                uniLines.Append(diffResult.PiecesNew[blockBPosition]);

            return uniLines.ToString();
        }
    }
}
