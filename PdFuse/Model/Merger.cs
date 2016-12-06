using iTextSharp.text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;

namespace PdFuse.Model
{
    public class Merger
    {
        private PdfReader _pdfReader;
        private Document _document;
        private PdfCopy _pdfCopy;
        private List<string> _sourceFiles;
        private string _resultPath;

        public Merger(List<string> sourceFiles, string resultPath)
        {
            _sourceFiles = new List<string>(sourceFiles);
            _resultPath = resultPath;
        }

        public void MergePdf()
        {
            using (FileStream fileStream = new FileStream(_resultPath, FileMode.Create))
            {
                _document = new Document();
                _pdfCopy = new PdfCopy(_document, fileStream);
                _document.Open();
                try
                {
                    foreach (string file in _sourceFiles)
                    {
                        _pdfReader = new PdfReader(file);

                        for (int page = 1; page <= _pdfReader.NumberOfPages; page++)
                            _pdfCopy.AddPage(_pdfCopy.GetImportedPage(_pdfReader, page));

                        _pdfReader.Close();
                    }
                }
                catch(InvalidPdfException invalidPdfException)
                {
                    //_pdfReader.Close();
                    _document.Close();
                }


                
            }
        }
    }
}
