using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace PdFuse.Model
{
    public abstract class Extractor
    {
        internal PdfReader pdfReader;
        internal Document document;
        internal PdfCopy pdfCopy;
        internal string resultFileName;
        internal string sourcePath;
        internal string resultFolderPath;
        internal string statusMessage;

        public Extractor(string sourcePath, string resultFolderPath)
        {
            this.sourcePath = sourcePath;
            resultFileName = @"\"
            + Path.GetFileNameWithoutExtension(this.sourcePath) + "_pag";

            this.resultFolderPath = resultFolderPath;
            statusMessage = string.Empty;
        }
    }
}
