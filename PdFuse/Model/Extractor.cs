using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace PdFuse.Model
{
    public class Extractor
    {
        private PdfReader _pdfReader;
        private Document _document;
        private PdfCopy _pdfCopy;
        private string _resultFileName;
        private string _sourcePath;
        private string _resultFolderPath;
        
        public Extractor(string sourcePath, string resultFolderPath)
        {
            _sourcePath = sourcePath;
            _resultFileName = @"\"
            + Path.GetFileNameWithoutExtension(_sourcePath) + "_pag";

            if (string.IsNullOrEmpty(resultFolderPath))
                _resultFolderPath = Path.GetDirectoryName(_sourcePath);
        }

        public void ExtractAllPages()
        {
           _pdfReader = new PdfReader(_sourcePath);

            for (int page = 1; page <= _pdfReader.NumberOfPages; page++)
            {
                _document = new Document();
                _pdfCopy =
                new PdfCopy(_document, new FileStream(_resultFolderPath
                    + _resultFileName + page + ".pdf", FileMode.Create));
                _document.Open();
                _pdfCopy.AddPage(_pdfCopy.GetImportedPage(_pdfReader, page));
                _document.Close();
            }

            _pdfReader.Close();
        }

        public void ExtractSelectedPages(string pageRanges)
        {
            foreach (string pageRange in pageRanges.Split(','))
            {
                if (pageRange.Contains("-"))
                    Extract(SplitType.Range, pageRange);

                else
                    Extract(SplitType.Specific, pageRange);
            }
        }

        private void Extract(SplitType splitType, string pageRange)
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

            using (FileStream fileStream = new FileStream(_resultFolderPath 
                + _resultFileName + pageName + ".pdf", FileMode.Create))
            {
                _pdfReader = new PdfReader(_sourcePath);
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

