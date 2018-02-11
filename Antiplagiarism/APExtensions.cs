using DiffPlex.Model;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Antiplagiarism
{
    public static class APExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        public static void ShowDifferences(this RichTextBox box, DiffResult diffResult)
        {
            Color normalColor = Color.Blue;
            Color normalColor2 = Color.Brown;
            Color insertColor = Color.Green;
            Color deleteColor = Color.Red;

            int blockBPosition = 0;

            for (int j = 0; j < diffResult.DiffBlocks.Count; j++)
            {
                var diffBlock = diffResult.DiffBlocks[j];

                //box.AppendText(Environment.NewLine + Environment.NewLine);

                //box.AppendText(Environment.NewLine + "\tBLOCK: " + Environment.NewLine);

                //box.AppendText(Environment.NewLine + "SAME PART 1: " + Environment.NewLine);
                for (; blockBPosition < diffBlock.InsertStartB; blockBPosition++)
                {
                    box.AppendText(diffResult.PiecesNew[blockBPosition], normalColor);
                }

                int i = 0;
                for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
                {
                    //box.AppendText(Environment.NewLine + "CHANGE PAIR: " + Environment.NewLine);
                    //box.AppendText(diffResult.PiecesOld[i + diffBlock.DeleteStartA] + " ", deleteColor);
                    //box.AppendText(diffResult.PiecesNew[i + diffBlock.InsertStartB], insertColor);

                    blockBPosition++;
                }

                if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
                {
                    //box.AppendText(Environment.NewLine + "DELETE A: " + Environment.NewLine);
                    for (; i < diffBlock.DeleteCountA; i++)
                    {
                        //box.AppendText(diffResult.PiecesOld[i + diffBlock.DeleteStartA], deleteColor);
                    }
                }
                else
                {
                    if (i < diffBlock.InsertCountB)
                    {
                        //box.AppendText(Environment.NewLine + "INSERT B: " + Environment.NewLine);
                        for (; i < diffBlock.InsertCountB; i++)
                        {
                            //box.AppendText(diffResult.PiecesNew[i + diffBlock.InsertStartB], insertColor);
                            blockBPosition++;
                        }
                    }
                }
            }

            //box.AppendText(Environment.NewLine + "SAME PART 2: " + Environment.NewLine);
            for (; blockBPosition < diffResult.PiecesNew.Length; blockBPosition++)
            {
                box.AppendText(diffResult.PiecesNew[blockBPosition], normalColor2);
            }
        }

        public static string RemoveWhiteSpaces(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

    }
}
