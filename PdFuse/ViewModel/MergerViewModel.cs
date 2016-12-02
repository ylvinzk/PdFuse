using System.Collections.Generic;
using System.Linq;
using PdFuse.Model;
using System.Windows.Controls;

namespace PdFuse.ViewModel
{
    public class MergerViewModel
    {
        private List<string> _sourceFiles;
        private string _resultPath;

        public MergerViewModel(ItemCollection sourceFiles, string resultPath)
        {
            _sourceFiles = new List<string>(sourceFiles.OfType<string>());
            _resultPath = resultPath;            
        }

        /// <summary>
        /// Calls the merge method in Merger.cs
        /// </summary>
        public void MergePdf()
        {
            Merger _merger = new Merger(_sourceFiles, _resultPath);
            _merger.MergePdf();
        }

        /// <summary>
        /// Checks for empty result path
        /// </summary>
        /// <returns></returns>
        public bool ResultPathIsEmpty()
        {
            return string.IsNullOrEmpty(_resultPath);
        }

        /// <summary>
        /// Checks for the same path in source and result
        /// </summary>
        /// <returns></returns>
        public bool ResultPathIsInUse()
        {
            return _sourceFiles.Exists(path => path.Equals(_resultPath));
        }
    }
}
