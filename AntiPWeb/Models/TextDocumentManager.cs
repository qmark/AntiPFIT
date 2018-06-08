using Spire.Doc;

namespace AntiPShared
{
    public class TextDocumentManager
    {
        public static string SimplifiedTextFromFile(string path)
        {
            var doc = new Document();
            doc.LoadFromFile(path);

            return TextManager.SimplifyText(doc.GetText());
        }

        public static string TextFromFile(string path)
        {
            var doc = new Document();
            doc.LoadFromFile(path);

            return doc.GetText();
        }
    }
}