using Microsoft.Win32;
using SeatingHelper.Model;
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
        private List<string> players = new List<string>();
        private List<Piece> importedPieces = new List<Piece>();
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
                PopulateMockData();
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

            numPlayers.Text = players.Count.ToString();
        }

        private void PopulateMockData()
        {
            Piece piece1 = new Piece() { Name = "Piece 1" };
            piece1.Assignments.Add(new Assignment("Player 1", "1"));
            piece1.Assignments.Add(new Assignment("Player 2", "3"));
            piece1.Assignments.Add(new Assignment("Player 3", "5"));
            piece1.Assignments.Add(new Assignment("Player 4", "1"));
            piece1.Assignments.Add(new Assignment("Player 5", "2"));
            piece1.Assignments.Add(new Assignment("Player 6", "3"));
            piece1.Assignments.Add(new Assignment("Player 7", "4"));
            piece1.Assignments.Add(new Assignment("Player 8", "1"));
            piece1.Assignments.Add(new Assignment("Player 9", "3"));
            piece1.Assignments.Add(new Assignment("Player 10", "2"));
            piece1.Assignments.Add(new Assignment("Player 11", "4"));
            piece1.Assignments.Add(new Assignment("Player 12", "1"));
            piece1.Assignments.Add(new Assignment("Player 13", "3"));
            piece1.Assignments.Add(new Assignment("Player 14", "5"));
            piece1.Assignments.Add(new Assignment("Player 15", "5"));
            piece1.Assignments.Add(new Assignment("Player 16", "2"));
            piece1.Assignments.Add(new Assignment("Player 17", "4"));
            piece1.Assignments.Add(new Assignment("Player 18", "5"));
            piece1.Assignments.Add(new Assignment("Player 19", "2"));
            piece1.Assignments.Add(new Assignment("Player 20", "4"));
            Piece piece2 = new Piece() { Name = "Piece 2" };
            piece2.Assignments.Add(new Assignment("Player 1", "7"));
            piece2.Assignments.Add(new Assignment("Player 2", "4"));
            piece2.Assignments.Add(new Assignment("Player 3", "1"));
            piece2.Assignments.Add(new Assignment("Player 4", "6"));
            piece2.Assignments.Add(new Assignment("Player 5", "4"));
            piece2.Assignments.Add(new Assignment("Player 6", "1"));
            piece2.Assignments.Add(new Assignment("Player 7", "7"));
            piece2.Assignments.Add(new Assignment("Player 8", "6"));
            piece2.Assignments.Add(new Assignment("Player 9", "1"));
            piece2.Assignments.Add(new Assignment("Player 10", "3"));
            piece2.Assignments.Add(new Assignment("Player 11", "4"));
            piece2.Assignments.Add(new Assignment("Player 12", "2"));
            piece2.Assignments.Add(new Assignment("Player 13", "5"));
            piece2.Assignments.Add(new Assignment("Player 14", "2"));
            piece2.Assignments.Add(new Assignment("Player 15", "3"));
            piece2.Assignments.Add(new Assignment("Player 16", "2"));
            piece2.Assignments.Add(new Assignment("Player 17", "5"));
            piece2.Assignments.Add(new Assignment("Player 18", "8"));
            piece2.Assignments.Add(new Assignment("Player 19", "3"));
            piece2.Assignments.Add(new Assignment("Player 20", "8"));
            players = piece1.Assignments.Select(a => a.PlayerName).Distinct().ToList();
            importedPieces.Add(piece1);
            importedPieces.Add(piece2);
        }
    }
}