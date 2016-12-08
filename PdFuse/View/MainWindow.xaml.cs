using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;
using Microsoft.Win32;
using System;
using System.IO;
using PdFuse.ViewModel;
using Ookii.Dialogs.Wpf;

namespace PdFuse.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();
        }

        #region General Controls

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TitleBarGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion

        #region Extract Tab

        private void SourceSearchButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            openFileDialog.Title = "Open source PDF document";
            if (openFileDialog.ShowDialog() == true)
            {
                SourcePathTextBox.Text = openFileDialog.FileName;
                if (string.IsNullOrEmpty(ResultFolderPathTextBox.Text))
                    ResultFolderPathTextBox.Text = Path.GetDirectoryName(openFileDialog.FileName) + @"\";
                ExtractStatusTextBlock.Text = string.Empty;
            }
        }

        private void ResultFolderSearchButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true)
            {
                ResultFolderPathTextBox.Text = 
                    folderBrowserDialog.SelectedPath + @"\"; 
                ExtractStatusTextBlock.Text = string.Empty;
            }                      
        }

        private void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            ExtractorViewModel extractorViewModel = 
                new ExtractorViewModel(SourcePathTextBox.Text,
                ResultFolderPathTextBox.Text, SelectedPagesRadioButton.IsChecked,
                PageSelectionTextBox.Text);

            if (!string.IsNullOrEmpty(extractorViewModel.StatusMessage))
            {
                ExtractStatusTextBlock.Text = extractorViewModel.StatusMessage;
                return;
            }

            ExtractStatusTextBlock.Text = "Extraction in progress...";
            extractorViewModel.Extract();
            ExtractStatusTextBlock.Text = extractorViewModel.StatusMessage;
        }

        private void SelectedPagesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            PageSelectionTextBox.IsEnabled = true;
            ExtractStatusTextBlock.Text = string.Empty;
        }

        private void SelectedPagesRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            PageSelectionTextBox.IsEnabled = false;
            ExtractStatusTextBlock.Text = string.Empty;
        }

        private void OpenResultsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ResultFolderPathTextBox.Text))
            {
                ExtractStatusTextBlock.Text = "Empty result path is invalid";
                return;
            }

            Process.Start(@"" + Path.GetDirectoryName(ResultFolderPathTextBox.Text));
            e.Handled = true;
        }

        #endregion

        #region Merge Tab

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

        private void SourceFilesListBox_Drop(object sender, DragEventArgs e)
        {
            AddPathsToList(e.Data.GetData(DataFormats.FileDrop) as string[]);
        }

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

            MergeStatusTextBlock.Text = string.Empty;
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (SourceFilesListBox.SelectedItems.Count > 0)
                OperateOnSelectedFile(FileOperation.MoveUp);
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (SourceFilesListBox.SelectedItems.Count > 0)
                OperateOnSelectedFile(FileOperation.MoveDown);
        }

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
                MergeStatusTextBlock.Text = string.Empty;
            }
        }

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            MergerViewModel _mergerViewModel = new MergerViewModel(SourceFilesListBox.Items, ResultFileTextBox.Text);

            if (!string.IsNullOrEmpty(_mergerViewModel.StatusMessage))
            {
                MergeStatusTextBlock.Text = _mergerViewModel.StatusMessage;
                return;
            }

            MergeStatusTextBlock.Text = "Merge in progress...";
            _mergerViewModel.Merge();
            MergeStatusTextBlock.Text = _mergerViewModel.StatusMessage;
        }

        private void OpenResultFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ResultFileTextBox.Text))
            {
                MergeStatusTextBlock.Text = "Empty result path is invalid";
                return;
            }

            Process.Start(@"" + Path.GetDirectoryName(ResultFileTextBox.Text));
            e.Handled = true;
        }

        private void OperateOnSelectedFile(FileOperation operation)
        {
            MergeStatusTextBlock.Text = string.Empty;

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

        private void EnableMoveButtons()
        {
            MoveDownButton.IsEnabled = true;
            MoveUpButton.IsEnabled = true;
        }

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