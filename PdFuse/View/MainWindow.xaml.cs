using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using PdFuse.ViewModel;

namespace PdFuse.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SplitterViewModel _splitterViewModel;
        private MergerViewModel _mergerViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region App General Controls
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

        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
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
        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            openFileDialog.AddExtension = true;
            openFileDialog.Title = "Add PDF documents";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                    SourceFilesListBox.Items.Add(fileName);
            }
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            object selectedPath = SourceFilesListBox.SelectedItem;
            int selectedPathIndex = SourceFilesListBox.Items.IndexOf(selectedPath);
            int nextPosition;

            if (selectedPathIndex == 0)
                nextPosition = SourceFilesListBox.Items.Count - 1;
            else
                nextPosition = selectedPathIndex - 1;

            SourceFilesListBox.Items.Remove(selectedPath);
            SourceFilesListBox.Items.Insert(nextPosition, selectedPath);
            SourceFilesListBox.SelectedIndex = nextPosition;
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                object selected = listBox1.SelectedItem;
                int indx = listBox1.Items.IndexOf(selected);
                int totl = listBox1.Items.Count;

                if (indx == totl - 1)
                {
                    listBox1.Items.Remove(selected);
                    listBox1.Items.Insert(0, selected);
                    listBox1.SetSelected(0, true);
                }
                else
                {
                    listBox1.Items.Remove(selected);
                    listBox1.Items.Insert(indx + 1, selected);
                    listBox1.SetSelected(indx + 1, true);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResultFileSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveFileDialog.AddExtension = true;
            saveFileDialog.FileName = "resultPDF_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            saveFileDialog.Title = "Path to the resulting PDF";
            saveFileDialog.OverwritePrompt = true;
            if (saveFileDialog.ShowDialog() == true && SourceFilesListBox.Items.Count > 0)
            {
                ResultFileTextBox.Text = saveFileDialog.FileName;
                AddFileButton.IsEnabled = true;
            }
        }

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}

