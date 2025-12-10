using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SeatingHelper
{
    /// <summary>
    /// Interaction logic for TemplatesDownloadWindow.xaml
    /// </summary>
    public partial class TemplatesDownloadWindow : Window
    {
        public TemplatesDownloadWindow()
        {
            InitializeComponent();
        }

        private void DownloadBasicButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadFile("SeatingTemplate.xlsx");
        }

        private void DownloadScoreOrderButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadFile("SeatingTemplateWithScoreOrder.xlsx");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DownloadFile(string inputFileName)
        {
            string tempFilePath = System.IO.Path.GetTempFileName();
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream? input = assembly.GetManifestResourceStream($"SeatingHelper.Templates.{inputFileName}"))
            using (Stream output = File.Create(tempFilePath))
            {
                if (input == null) return;
                input.CopyTo(output);
            }
            var saveDialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = inputFileName
            };
            if (saveDialog.ShowDialog() == true)
            {
                File.Copy(tempFilePath, saveDialog.FileName, true);
            }

            File.Delete(tempFilePath);
        }
    }
}
