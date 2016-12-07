using System.Collections.Generic;
using System.Linq;
using PdFuse.Model;
using System.Windows.Controls;

namespace PdFuse.ViewModel
{
    public class MergerViewModel
    {
        private Merger _merger;
        private List<string> _sourceFiles;
        private string _resultPath;

        public string StatusMessage { get; private set; }

        public MergerViewModel(ItemCollection sourceFiles, string resultPath)
        {
            _sourceFiles = new List<string>(sourceFiles.OfType<string>());
            _resultPath = resultPath;
            StatusMessage = string.Empty;

            CheckValidity();
        }

        /// <summary>
        /// Check for validation to merge
        /// </summary>
        public void CheckValidity()
        {
            if (_sourceFiles.Count < 2)
            {
                StatusMessage = "Not enough documents to merge";
                return;
            }

            if (string.IsNullOrEmpty(_resultPath))
            {
                StatusMessage = "Empty result path is invalid";
                return;
            }

            if (_sourceFiles.Exists(path => path.Equals(_resultPath)))
            {
                StatusMessage = "Can not use same path in source and result";
                return;
            }

        }

        /// <summary>
        /// Calls the merge method in Merger.cs
        /// </summary>
        public void MergePdf()
        {
            _merger = new Merger(_sourceFiles, _resultPath);

            _merger.MergePdf();
            StatusMessage = _merger.StatusMessage;
        }
    }
}
