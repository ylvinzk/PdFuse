using PdFuse.Model;
using System.IO;
using System.Text.RegularExpressions;

namespace PdFuse.ViewModel
{
    public class ExtractorViewModel
    {
        private string _sourcePath;
        private string _resultsFolderPath;
        private bool? _extractBySelection;
        private string _pageSelection;
        private string _resultFileName;

        public string StatusMessage { get; private set; }

        public ExtractorViewModel(string sourcePath, string resultsFolderPath,
            bool? extractBySelection, string PageSelection)
        {
            _sourcePath = sourcePath;
            _resultsFolderPath = resultsFolderPath;
            StatusMessage = string.Empty;
            _extractBySelection = extractBySelection;
            _pageSelection = PageSelection;

            CheckPathValidity();
        }

        private void CheckPathValidity()
        {
            if (string.IsNullOrEmpty(_sourcePath))
            {
                StatusMessage = "An empty source path is invalid";
                return;
            }

            string pageRangesPattern =
                "(?!(([1-9]{1}[0-9]*)-(([1-9]{1}[0-9]*))-))"
                + "^(([1-9]{1}[0-9]*)|(([1-9]{1}[0-9]*)((,?([1-9]{1}[0-9]*))|"
                + "(-?([1-9]{1}[0-9]*)){1})*))$";
            //"(?!(([1-9]{1}[0-9]*)-(([1-9]{1}[0-9]*))-))^(([1-9]{1}[0-9]*)|(([1-9]{1}[0-9]*)((,?([1-9]{1}[0-9]*))|(-?([1-9]{1}[0-9]*)){1})*))$";

            if (_extractBySelection == true
                && !Regex.IsMatch(_pageSelection, pageRangesPattern))
            {
                StatusMessage = "Invalid page selection";
                return;
            }
        }

        public void Extract()
        {
            Extractor _extractor = new Extractor(_sourcePath, _resultsFolderPath);

            if (_extractBySelection == true)
                _extractor.ExtractSelectedPages(_pageSelection);
            else
                _extractor.ExtractAllPages();
        }
    }
}
