using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;
using Microsoft.Win32;
using System;
using System.IO;
using PdFuse.ViewModel;

namespace PdFuse.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MergerViewModel _mergerViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region General Controls

        /// <summary>
        /// Close app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Minimize window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Drag window by the title bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBarGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion

        #region Split Tab
        //private void PdfSourceSearchButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ProgressStatusTextBlock.Text = string.Empty;

        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        _splitter = new Splitter(openFileDialog.FileName);
        //        PdfSourcePathTextBox.Text = _splitter.SourcePath;
        //        PdfDestinationPathTextBox.Text = _splitter.ResultPath;
        //        SplitButton.IsEnabled = true;
        //        PdfDestinationSearchButton.IsEnabled = true;
        //    }
        //}

        //private void PdfDestinationSearchButton_Click(object sender, RoutedEventArgs e)
        //{
        //    VistaFolderBrowserDialog outputPathDialog = new VistaFolderBrowserDialog();
        //    if (outputPathDialog.ShowDialog() == true)
        //        PdfDestinationPathTextBox.Text = _splitter.SetResultPath(outputPathDialog.SelectedPath);
        //}

        //private void SplitButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (AllPagesRadioButton.IsChecked == true)
        //        _splitter.SplitAllPages();
        //    else
        //        _splitter.SplitSelectedPages(PageSelectionTextBox.Text);
        //}

        //private void SelectedPagesRadioButton_Checked(object sender, RoutedEventArgs e)
        //{
        //    PageSelectionStackPanel.Visibility = Visibility.Visible;
        //}

        //private void SelectedPagesRadioButton_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    PageSelectionStackPanel.Visibility = Visibility.Hidden;
        //}
        #endregion

        #region Merge Tab

        /// <summary>
        /// Add PDF files to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            openFileDialog.AddExtension = true;
            openFileDialog.Title = "Add PDF documents";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
                AddPathsToList(openFileDialog.FileNames);
        }

        /// <summary>
        /// Filter dragged files to the SourceFilesListBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceFilesListBox_DragOver(object sender, DragEventArgs e)
        {
            bool dropEnabled = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = e.Data.GetData(DataFormats.FileDrop) as string[];

                foreach (string filename in filenames)
                    if (Path.GetExtension(filename).ToLowerInvariant() != ".pdf")
                    {
                        dropEnabled = false;
                        break;
                    }
            }

            if (!dropEnabled)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Get the dragged file paths 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceFilesListBox_Drop(object sender, DragEventArgs e)
        {
            AddPathsToList(e.Data.GetData(DataFormats.FileDrop) as string[]);
        }

        /// <summary>
        /// Add the chosen file paths to the list
        /// </summary>
        /// <param name="fileNames"></param>
        private void AddPathsToList(string[] fileNames)
        {
            foreach (string fileName in fileNames)
                SourceFilesListBox.Items.Add(fileName);

            if (SourceFilesListBox.Items.Count > 0)
            {
                DeleteButton.IsEnabled = true;

                if (SourceFilesListBox.Items.Count > 1)
                    EnableMoveButtons();
            }

            ProcessStatusTextBlock.Text = string.Empty;
        }

        /// <summary>
        /// Move up a file path in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (SourceFilesListBox.SelectedItems.Count > 0)
                OperateOnSelectedFile(FileOperation.MoveUp);
        }

        /// <summary>
        /// Move down a file path in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (SourceFilesListBox.SelectedItems.Count > 0)
                OperateOnSelectedFile(FileOperation.MoveDown);
        }

        /// <summary>
        /// Delete a file path in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SourceFilesListBox.SelectedItems.Count > 0)
                OperateOnSelectedFile(FileOperation.Delete);

            if (SourceFilesListBox.Items.Count < 2)
            {
                DisableMoveButtons();
                if (SourceFilesListBox.Items.Count == 0)
                    DeleteButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Define the result path to save the merged file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResultFileSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveFileDialog.AddExtension = true;
            saveFileDialog.FileName = "resultPDF_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            saveFileDialog.Title = "Path to the resulting PDF";
            saveFileDialog.OverwritePrompt = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                ResultFileTextBox.Text = saveFileDialog.FileName;
                ProcessStatusTextBlock.Text = string.Empty;
            }
        }

        /// <summary>
        /// Start the merging process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            _mergerViewModel = new MergerViewModel(SourceFilesListBox.Items, ResultFileTextBox.Text);

            if (!string.IsNullOrEmpty(_mergerViewModel.StatusMessage))
            {
                ProcessStatusTextBlock.Text = _mergerViewModel.StatusMessage;
                return;
            }

            ProcessStatusTextBlock.Text = "Merge in progress...";
            _mergerViewModel.MergePdf();
            ProcessStatusTextBlock.Text = _mergerViewModel.StatusMessage;
        }

        /// <summary>
        /// Open the directory to the result file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenResultFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ResultFileTextBox.Text))
            {
                ProcessStatusTextBlock.Text = "Empty result path is invalid";
                return;
            }

            Process.Start(@"" + Path.GetDirectoryName(ResultFileTextBox.Text));
            e.Handled = true;
        }

        /// <summary>
        /// Apply an operation to a file path in the list
        /// </summary>
        /// <param name="operation"></param>
        private void OperateOnSelectedFile(FileOperation operation)
        {
            ProcessStatusTextBlock.Text = string.Empty;

            int selectedPathIndex = SourceFilesListBox.SelectedIndex;
            object selectedPath = SourceFilesListBox.Items.GetItemAt(selectedPathIndex);
            int totalItems = SourceFilesListBox.Items.Count;
            int nextPosition;

            switch (operation)
            {
                case FileOperation.MoveUp:
                    if (selectedPathIndex == 0)
                        nextPosition = totalItems - 1;
                    else
                        nextPosition = selectedPathIndex - 1;
                    break;
                case FileOperation.MoveDown:
                    if (selectedPathIndex == totalItems - 1)
                        nextPosition = 0;
                    else
                        nextPosition = selectedPathIndex + 1;
                    break;
                default:
                    if (selectedPathIndex == totalItems - 1)
                        nextPosition = 0;
                    else
                        nextPosition = selectedPathIndex;
                    SourceFilesListBox.Items.RemoveAt(selectedPathIndex);
                    SourceFilesListBox.SelectedIndex = nextPosition;
                    return;
            }

            SourceFilesListBox.Items.RemoveAt(selectedPathIndex);
            SourceFilesListBox.Items.Insert(nextPosition, selectedPath);
            SourceFilesListBox.SelectedIndex = nextPosition;
        }

        /// <summary>
        /// Enable Move buttons
        /// </summary>
        private void EnableMoveButtons()
        {
            MoveDownButton.IsEnabled = true;
            MoveUpButton.IsEnabled = true;
        }

        /// <summary>
        /// Disable Move buttons
        /// </summary>
        private void DisableMoveButtons()
        {
            MoveDownButton.IsEnabled = false;
            MoveUpButton.IsEnabled = false;
        }

        #endregion

        #region Help Tab

        /// <summary>
        /// Navigation to an URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        #endregion

    }
}

