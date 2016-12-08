using iTextSharp.text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;

namespace PdFuse.Model
{
    public class Merger
    {
        private List<string> _sourceFiles;
        private string _resultPath;
        private string actualFileName;

        public string StatusMessage { get; private set; }

        public Merger(List<string> sourceFiles, string resultPath)
        {
            _sourceFiles = new List<string>(sourceFiles);
            _resultPath = resultPath;
            StatusMessage = string.Empty;
        }

        public void MergePdf()
        {
            try
            {
                using (FileStream fileStream = new FileStream(_resultPath, FileMode.Create))
                {
                    Document document = new Document();
                    PdfCopy pdfCopy = new PdfCopy(document, fileStream);
                    document.Open();

                    foreach (string file in _sourceFiles)
                    {
                        actualFileName = Path.GetFileNameWithoutExtension(file);
                        PdfReader pdfReader = new PdfReader(file);

                        for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                            pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, page));

                        pdfReader.Close();
                    }

                    document.Close();
                    StatusMessage = "Merge is complete";
                }
            }
            catch (InvalidPdfException invalidPdfException)
            {
                StatusMessage = "Damaged : " + actualFileName;
                File.Delete(_resultPath);
            }
        }
    }
}
