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
            PopulateMockData();
        }

        private void Button_Choose_File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Select a File";
            openFileDialog.Filter = "*.xls|*.xlsx";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Multiselect = false; 

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFileName = openFileDialog.FileName;

                filenameDisplay.Text = selectedFileName;
                CountPlayers(selectedFileName);
                chartsList.Items.Clear();
                foreach (Piece piece in importedPieces)
                {
                    bool success = SeatingCalculation.TryLongerRowsPieceSeating(piece, numRows.Value ?? 0, out Assignment[][] seating);
                    if (success) PopulateListView(seating);
                }
                bool exampleSuccess = SeatingCalculation.TrySimplePieceSeating(importedPieces[1], numRows.Value ?? 0, out Assignment[][] displaySeatingExample);
                if (exampleSuccess) DisplayPieceSeating(displaySeatingExample);
                else MessageBox.Show($"Parts don't fit cleanly into {numRows.Value ?? 0} rows.");
            }
            else
            {
                MessageBox.Show("File selection cancelled.");
            }
        }

        private void PopulateListView(Assignment[][] seating)
        {
            chartsList.Items.Add(new { Rows = seating.Length, Players = seating.LongLength, Chart = seating });
        }

        private void DisplayPieceSeating(Assignment[][] seating)
        {
            StringBuilder displaySB = new StringBuilder();
            for (int i = 0; i < seating.Length; i++) // row
            {
                displaySB.Append($"Row {i + 1}: ");
                for (int j = 0; j < seating[i].Length; j++) // seat
                {
                    displaySB.Append($"  Name: {seating[i][j].PlayerName}, Part: {seating[i][j].PartName}  ");
                }
                displaySB.AppendLine();
            }
            DisplayWindow display = new(displaySB.ToString());
            display.Show();
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
            piece2.Assignments.Add(new Assignment("Player 1",  "8"));
            piece2.Assignments.Add(new Assignment("Player 2",  "1"));
            piece2.Assignments.Add(new Assignment("Player 3",  "3"));
            piece2.Assignments.Add(new Assignment("Player 4",  "6"));
            piece2.Assignments.Add(new Assignment("Player 5",  "1"));
            piece2.Assignments.Add(new Assignment("Player 6",  "4"));
            piece2.Assignments.Add(new Assignment("Player 7",  "6"));
            piece2.Assignments.Add(new Assignment("Player 8",  "1"));
            piece2.Assignments.Add(new Assignment("Player 9",  "4"));
            piece2.Assignments.Add(new Assignment("Player 10", "7"));
            piece2.Assignments.Add(new Assignment("Player 11", "2"));
            piece2.Assignments.Add(new Assignment("Player 12", "4"));
            piece2.Assignments.Add(new Assignment("Player 13", "7"));
            piece2.Assignments.Add(new Assignment("Player 14", "2"));
            piece2.Assignments.Add(new Assignment("Player 15", "5"));
            piece2.Assignments.Add(new Assignment("Player 16", "7"));
            piece2.Assignments.Add(new Assignment("Player 17", "2"));
            piece2.Assignments.Add(new Assignment("Player 18", "5"));
            piece2.Assignments.Add(new Assignment("Player 19", "8"));
            piece2.Assignments.Add(new Assignment("Player 20", "3"));
            piece2.Assignments.Add(new Assignment("Player 21", "5"));
            piece2.Assignments.Add(new Assignment("Player 22", "8"));
            piece2.Assignments.Add(new Assignment("Player 23", "3"));
            piece2.Assignments.Add(new Assignment("Player 24", "6"));
            importedPieces.Add(piece1);
            importedPieces.Add(piece2);
            players = importedPieces
                        .SelectMany(p => p.Assignments)
                        .Select(a => a.PlayerName)
                        .Distinct()
                        .ToList();
            UpdateMaxRowWidth();
        }

        private void numRows_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateMaxRowWidth();
        }

        private void UpdateMaxRowWidth()
        {
            if (maxRowWidth != null)
            {
                maxRowWidth.Minimum = players.Count / numRows.Value;
                if (maxRowWidth.Value < maxRowWidth.Minimum) maxRowWidth.Value = maxRowWidth.Minimum;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}