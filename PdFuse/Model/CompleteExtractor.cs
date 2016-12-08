using iTextSharp.text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using System.IO;

namespace PdFuse.Model
{
    /// <summary>
    /// Extractor for all pages
    /// </summary>
    public class CompleteExtractor : Extractor
    {
        public CompleteExtractor(string sourcePath, string resultFolderPath)
            : base(sourcePath, resultFolderPath) { }

        internal void Extract()
        {
            try
            {
                pdfReader = new PdfReader(sourcePath);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    document = new Document();
                    pdfCopy =
                    new PdfCopy(document, new FileStream(resultFolderPath 
                    + resultFileName + page + ".pdf", FileMode.Create));
                    document.Open();
                    pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, page));
                    document.Close();
                }

                pdfReader.Close();
                statusMessage = "Extraction complete.";
            }
            catch (InvalidPdfException)
            {
                statusMessage = "Damaged source file.";
            }
        }
    }
}