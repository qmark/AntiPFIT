using Spire.Doc;

namespace AntiPShared
{
    public class TextDocumentManager
    {
        public static string SimplifiedTextFromFile(string fileName)
        {
            var doc = new Document();
            doc.LoadFromFile(fileName);

            return TextManager.SimplifyText(doc.GetText()).Replace("\r\n", " ");
        }
        public static string TextFromFile(string fileName)
        {
            var doc = new Document();
            doc.LoadFromFile(fileName);

            return doc.GetText();
        }
    }
}
