using iTextSharp.text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace PdFuse.Model
{
    public class PartialExtractor : Extractor
    {
        public PartialExtractor(string sourcePath, string resultFolderPath)
            : base(sourcePath, resultFolderPath){ }

        internal void Extract(string pageSelection)
        {
            int firstPage;
            int lastPage;
            string pageName;
            
            foreach (string pageRange in pageSelection.Split(','))
            {
                if (pageRange.Contains("-"))
                {
                    firstPage = int.Parse(pageRange.Split('-')[0]);
                    lastPage = int.Parse(pageRange.Split('-')[1]);
                    pageName = firstPage + "to" + lastPage;
                }

                else
                {
                    firstPage = int.Parse(pageRange);
                    lastPage = firstPage;
                    pageName = firstPage.ToString();
                }

                string resultPath = resultFolderPath + resultFileName + pageName + ".pdf";

                try
                {                    
                    using (FileStream fileStream = new FileStream(resultPath, FileMode.Create))
                    {
                        pdfReader = new PdfReader(sourcePath);
                        document = new Document();
                        pdfCopy = new PdfCopy(document, fileStream);
                        document.Open();

                        for (int page = firstPage; page <= lastPage; page++)
                            pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, page));

                        document.Close();
                        pdfReader.Close();
                    }                    
                }
                catch (ArgumentException)
                {
                    statusMessage = "Invalid selection. Page number is greater than the document.";
                    File.Delete(resultPath);
                    return;
                }
                catch (InvalidPdfException)
                {
                    statusMessage = "Damaged source file.";
                    File.Delete(resultPath);
                    return;
                }
            }

            statusMessage = "Extraction complete.";
        }
    }
}

