using System.Collections.Generic;
using System.Linq;
using PdFuse.Model;
using System.Windows.Controls;

namespace PdFuse.ViewModel
{
    public class MergerViewModel
    {
        private List<string> _sourceFiles;
        private Merger _merger;

        public MergerViewModel(ItemCollection sourceFiles, string resultPath)
        {
            _sourceFiles = new List<string>(sourceFiles.OfType<string>());
            _merger = new Merger(_sourceFiles, resultPath);
        }

        public void MergePdf()
        {
            _merger.MergePdf();
        }
    }
}
