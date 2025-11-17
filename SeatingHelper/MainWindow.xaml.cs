using Microsoft.Win32;
using SeatingHelper.Model;
using System.Collections.ObjectModel;
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
using Xceed.Wpf.Toolkit;
using OfficeOpenXml;

namespace SeatingHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> players = new List<string>();
        private List<Piece> importedPieces = new List<Piece>();
        private List<Assignment[][]> seatingCharts = new List<Assignment[][]>();
        public MainWindow()
        {
            InitializeComponent();
            //PopulateMockData();
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
                string filepath = openFileDialog.FileName;
                ParseSheets(filepath);
                players = importedPieces
                            .SelectMany(p => p.Assignments)
                            .Select(a => a.PlayerName)
                            .Distinct()
                            .ToList();
                CountPlayers();
                UpdateMaxRowWidth();
            }
            else
            {
                System.Windows.MessageBox.Show("File selection cancelled.");
            }
        }

        private void PopulateListView(Assignment[][] seating)
        {
            chartsList.Items.Add(new ChartListViewItem(seating));
        }

        private void DisplayPieceSeating(Assignment[][] seating)
        {
            StringBuilder displaySB = new StringBuilder();
            for (int i = 0; i < seating.Length; i++) // row
            {
                displaySB.Append($"Row {i + 1}: ");
                for (int j = 0; j < seating[i].Length; j++) // seat
                {
                    displaySB.Append($"  [{seating[i][j].PartName}]: {seating[i][j].PlayerName}  ");
                }
                displaySB.AppendLine();
            }
            DisplayWindow display = new(displaySB.ToString());
            display.Show();
        }

        private void CountPlayers()
        {
            numPlayers.Text = players.Count.ToString();
        }

        private void ParseSheets(string filepath)
        {
            importedPieces.Clear();
            ExcelPackage.License.SetNonCommercialPersonal("Ryan Luttrull");
            using (var package = new ExcelPackage(filepath))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                for (int col = 2; col <= worksheet.Columns.Count(); col++)
                {
                    Piece pieceToAdd = new Piece();
                    pieceToAdd.Name = (string)worksheet.Cells[1, col].Value;
                    for (int row = 2; row <= worksheet.Rows.Count(); row++)
                    {
                        if (worksheet.Cells[row, col].Value is null) continue;
                        string playerName = worksheet.Cells[row, 1].Value.ToString();
                        string partName = worksheet.Cells[row, col].Value.ToString();
                        pieceToAdd.Assignments.Add(new Assignment(playerName, partName));
                    }
                    importedPieces.Add(pieceToAdd);
                }
            }
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

        private void ListViewItem_DoubleClick(object sender, RoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                ChartListViewItem chartItem = item.Content as ChartListViewItem;
                if (chartItem?.Chart != null)
                {
                    DisplayPieceSeating(chartItem.Chart);
                }
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            chartsList.Items.Clear();

            foreach (Piece piece in importedPieces)
            {
                bool blockSuccess, straightSuccess;
                if (tryBlockFirst.IsChecked == true)
                {
                    blockSuccess = SeatingCalculation.TryBlockPieceSeating(piece, numRows.Value ?? 0, maxRowWidth.Value ?? 0, out Assignment[][] blockSeating);
                    if (blockSuccess)
                    {
                        PopulateListView(blockSeating);
                        continue;
                    }
                    straightSuccess = SeatingCalculation.TryLongerRowsPieceSeating(piece, numRows.Value ?? 0, out Assignment[][] straightSeating);
                    if (straightSuccess)
                    {
                        PopulateListView(straightSeating);
                        continue;
                    }
                }
                else
                {
                    straightSuccess = SeatingCalculation.TryLongerRowsPieceSeating(piece, numRows.Value ?? 0, out Assignment[][] straightSeating);
                    if (straightSuccess)
                    {
                        PopulateListView(straightSeating);
                        continue;
                    }
                    blockSuccess = SeatingCalculation.TryBlockPieceSeating(piece, numRows.Value ?? 0, maxRowWidth.Value ?? 0, out Assignment[][] blockSeating);
                    if (blockSuccess)
                    {
                        PopulateListView(blockSeating);
                        continue;
                    }
                }
            }
        }
    }
}