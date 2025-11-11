using Microsoft.Win32;
using System.Formats.Tar;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace SeatingHelper
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

        private void Button_Choose_File_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Configure OpenFileDialog properties
            openFileDialog.Title = "Select a File";
            openFileDialog.Filter = "*.xls|*.xlsx";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Multiselect = false; // Set to true to allow multiple file selection

            // Show the dialog and get the result
            bool? result = openFileDialog.ShowDialog();

            // Process the dialog result
            if (result == true)
            {
                // Get the selected file name
                string selectedFileName = openFileDialog.FileName;

                // You can now use 'selectedFileName' to open and process the file
                MessageBox.Show($"Selected file: {selectedFileName}");
                filenameDisplay.Text = selectedFileName;
                CountPlayers(selectedFileName);
            }
            else
            {
                MessageBox.Show("File selection cancelled.");
            }
        }

        private void CountPlayers(string filepath)
        {
            //Excel.Application excelApp = new Excel.Application();
            //Excel.Workbook excelWorkbook = excelApp.Workbooks.Open(filepath);
            //Excel._Worksheet excelSheet = (Excel._Worksheet)excelWorkbook.Sheets[1]; // access the first sheet
            //Excel.Range excelRange = excelSheet.UsedRange; // get the used range of cells

            //numPlayers.Text = (excelRange.Rows.Count - 1).ToString();

            var rows = 21;
            numPlayers.Text = (rows - 1).ToString();
        }
    }
}