using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tesseract;

namespace AntiPShared
{
    public class OCR
    {
        public static async Task<string> GetTextFromPic(string testImagePath = "dou.PNG")
        {
            string result = string.Empty;

            var _wordsFromPic = new List<string>();

            using (var engine = new TesseractEngine(@"tessdata", "ukr+eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(testImagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        //var text = page.GetText();
                        //result += $"Mean confidence: {page.GetMeanConfidence()}" + Environment.NewLine;
                        //result += $"Text (GetText):{Environment.NewLine}{text}" + Environment.NewLine;
                        //result += Environment.NewLine;
                        //result += "=================================" + Environment.NewLine;
                        //result += Environment.NewLine;
                        //result += $"Text (iterator):" + Environment.NewLine;

                        using (var iter = page.GetIterator())
                        {
                            iter.Begin();

                            do
                            {
                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                            {
                                                //result += $"<BLOCK>" + Environment.NewLine;
                                                //result += Environment.NewLine;
                                            }

                                            var word = iter.GetText(PageIteratorLevel.Word);

                                            result += $"{word} ";

                                            _wordsFromPic.Add(word);

                                            //if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                            //{
                                            //result += Environment.NewLine;
                                            //}
                                        } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                                        if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                                        {
                                            result += Environment.NewLine;
                                        }
                                    } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                            } while (iter.Next(PageIteratorLevel.Block));
                        }
                    }
                }
            }

            return result;
        }

    }
}
