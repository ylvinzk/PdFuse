using PdFuse.Model;
using System.Text.RegularExpressions;

namespace PdFuse.ViewModel
{
    public class ExtractorViewModel
    {
        private string _sourcePath;
        private string _resultsFolderPath;
        private bool? _extractBySelection;
        private string _pageSelection;

        public string StatusMessage { get; private set; }

        public ExtractorViewModel(string sourcePath, string resultsFolderPath,
            bool? extractBySelection, string PageSelection)
        {
            _sourcePath = sourcePath;
            _resultsFolderPath = resultsFolderPath;
            StatusMessage = string.Empty;
            _extractBySelection = extractBySelection;
            _pageSelection = PageSelection;

            CheckInputValidity();
        }

        private void CheckInputValidity()
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

            if (_extractBySelection == true
                && !Regex.IsMatch(_pageSelection, pageRangesPattern))
            {
                StatusMessage = "Invalid page selection. Follow the guide";
                return;
            }
            else
            {
                foreach (string pageRange in _pageSelection.Split(','))
                {
                    if (pageRange.Contains("-")
                        && (int.Parse(pageRange.Split('-')[0]) > int.Parse(pageRange.Split('-')[1])))
                    {
                        StatusMessage = "Invalid page selection. First page of range is greater than last page";
                        return;
                    }
                }
                    
            }
        }

        public void Extract()
        {
            if (_extractBySelection == true)
            {
                PartialExtractor partialExtractor = new PartialExtractor(_sourcePath, _resultsFolderPath);
                partialExtractor.Extract(_pageSelection);
                StatusMessage = partialExtractor.statusMessage;
            }

            else
            {
                CompleteExtractor completeExtractor = new CompleteExtractor(_sourcePath, _resultsFolderPath);
                completeExtractor.Extract();
                StatusMessage = completeExtractor.statusMessage;
            }
        }
    }
}
