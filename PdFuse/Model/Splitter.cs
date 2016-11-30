using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text.RegularExpressions;

namespace PdFuse.Model
{
    public class Splitter
    {
        private PdfReader _pdfReader;
        private Document _document;
        private PdfCopy _pdfCopy;
        private string _resultFileName;
        private string _pageRangesPattern =
            "(?!(([1-9]{1}[0-9]*)-(([1-9]{1}[0-9]*))-))^(([1-9]{1}[0-9]*)|(([1-9]{1}[0-9]*)((,?([1-9]{1}[0-9]*))|(-?([1-9]{1}[0-9]*)){1})*))$";

        public string SourcePath { get; private set; }
        public string ResultPath { get; private set; }
        

        public Splitter(string sourcePath)
        {
            SourcePath = sourcePath;
            _resultFileName = @"\"
            + Path.GetFileNameWithoutExtension(SourcePath) + "_pag";
            ResultPath = Path.GetDirectoryName(SourcePath);
        }

        internal string SetResultPath(string resultPath)
        {
            return ResultPath = resultPath;
        }

        internal void SplitAllPages()
        {
           _pdfReader = new PdfReader(SourcePath);

            for (int page = 1; page <= _pdfReader.NumberOfPages; page++)
            {
                _document = new Document();
                _pdfCopy =
                new PdfCopy(_document, new FileStream(ResultPath
                    + _resultFileName + page + ".pdf", FileMode.Create));
                _document.Open();
                _pdfCopy.AddPage(_pdfCopy.GetImportedPage(_pdfReader, page));
                _document.Close();
            }

            _pdfReader.Close();
        }

        internal void SplitSelectedPages(string pageRanges)
        {            
            if (Regex.IsMatch(pageRanges, _pageRangesPattern))              
                foreach (string pageRange in pageRanges.Split(','))
                {                    
                    if (pageRange.Contains("-"))
                        SplitPdf(SplitType.Range, pageRange);
                           
                    else
                        SplitPdf(SplitType.Specific, pageRange);
                }
        }

        private void SplitPdf(SplitType splitType, string pageRange)
        {            
            int firstPage;
            int lastPage;
            string pageName;

            switch (splitType)
            {
                case SplitType.Range:
                    firstPage = int.Parse(pageRange.Split('-')[0]);
                    lastPage = int.Parse(pageRange.Split('-')[1]);
                    pageName = firstPage + "to" + lastPage;
                    break;
                default:
                    firstPage = int.Parse(pageRange);
                    lastPage = firstPage;
                    pageName = firstPage.ToString();
                    break;
            }

            using (FileStream fileStream = new FileStream(ResultPath 
                + _resultFileName + pageName + ".pdf", FileMode.Create))
            {
                _pdfReader = new PdfReader(SourcePath);
                _document = new Document();
                _pdfCopy = new PdfCopy(_document, fileStream);
                _document.Open();

                for (int page = firstPage; page <= lastPage; page++)
                    _pdfCopy.AddPage(_pdfCopy.GetImportedPage(_pdfReader, page));
                
                _document.Close();
                _pdfReader.Close();
            }
        }
    }
}

