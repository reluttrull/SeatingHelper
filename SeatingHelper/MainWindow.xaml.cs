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
using System.IO.Packaging;

namespace SeatingHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> players = new List<string>();
        private List<string> scoreOrder = new List<string>();
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
            openFileDialog.Filter = "Excel Sheet (*.xls, *.xlsx)|*.xls;*.xlsx";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Multiselect = false; 

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                exportButton.IsEnabled = false;
                string filepath = openFileDialog.FileName;
                filenameDisplay.Text = filepath;
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
                bool hasPriority = false;
                int col = 1;
                if (worksheet.Cells[1, 2].GetValue<string>().Equals("PRIORITY (OPTIONAL)", StringComparison.CurrentCultureIgnoreCase))
                {
                    hasPriority = true;
                    col++;
                }
                while (col + 1 <= worksheet.Columns.Count())
                {
                    col++;
                    Piece pieceToAdd = new Piece();
                    pieceToAdd.Name = (string)worksheet.Cells[1, col].Value;
                    for (int row = 2; row <= worksheet.Rows.Count(); row++)
                    {
                        if (worksheet.Cells[row, col].Value is null) continue;
                        string playerName = worksheet.Cells[row, 1].GetValue<string>();
                        string partName = worksheet.Cells[row, col].GetValue<string>();
                        int priority = hasPriority && worksheet.Cells[row,2].Value is not null ? worksheet.Cells[row, 2].GetValue<int>() : Int32.MaxValue;
                        pieceToAdd.Assignments.Add(new Assignment(playerName, partName, priority));
                    }
                    importedPieces.Add(pieceToAdd);
                }

                scoreOrder.Clear();
                ExcelWorksheet scoreOrderWorksheet = package.Workbook.Worksheets.Where(w => w.Name.Trim().Equals("SCORE ORDER", StringComparison.CurrentCultureIgnoreCase)).First();
                if (scoreOrderWorksheet is not null)
                {
                    for (int row = 1; row <= scoreOrderWorksheet.Rows.Count(); row++)
                    {
                        scoreOrder.Add(scoreOrderWorksheet.Cells[row, 1].GetValue<string>());
                    }
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
            seatingCharts.Clear();
            chartsList.Items.Clear();

            foreach (Piece piece in importedPieces)
            {
                SeatingCalculator seatingCalculator = new SeatingCalculator(piece, numRows.Value ?? 0, maxRowWidth.Value ?? 0);
                bool blockSuccess, straightSuccess;
                if (tryBlockFirst.IsChecked == true)
                {
                    blockSuccess = seatingCalculator.TryBlockPieceSeating(out Assignment[][] blockSeating);
                    if (blockSuccess)
                    {
                        seatingCharts.Add(blockSeating);
                        PopulateListView(blockSeating);
                        continue;
                    }
                    straightSuccess = seatingCalculator.TryLongerRowsPieceSeating(out Assignment[][] straightSeating);
                    if (straightSuccess)
                    {
                        seatingCharts.Add(straightSeating);
                        PopulateListView(straightSeating);
                        continue;
                    }
                }
                else
                {
                    straightSuccess = seatingCalculator.TryLongerRowsPieceSeating(out Assignment[][] straightSeating);
                    if (straightSuccess)
                    {
                        seatingCharts.Add(straightSeating);
                        PopulateListView(straightSeating);
                        continue;
                    }
                    blockSuccess = seatingCalculator.TryBlockPieceSeating(out Assignment[][] blockSeating);
                    if (blockSuccess)
                    {
                        seatingCharts.Add(blockSeating);
                        PopulateListView(blockSeating);
                        continue;
                    }
                }
            }
            exportButton.IsEnabled = true;
        }

        private ExcelPackage GenerateExcelPackage(List<Assignment[][]> seatingCharts)
        {
            ExcelPackage package = new ExcelPackage();
            for (int i = 0; i < seatingCharts.Count; i++)
            {
                Assignment[][] seatingChart = seatingCharts[i];
                ExcelWorksheet ws = package.Workbook.Worksheets.Add($"\"{importedPieces[i].Name}\"");
                for (int row = 0; row < seatingChart.Length; row++)
                {
                    for (int col = 0; col < seatingChart[row].Length; col++)
                    {
                        ws.Cells[row + 1, col + 1].Value = $"{seatingChart[row][col].PartName}: {seatingChart[row][col].PlayerName}";
                    }
                }
            }
            return package;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                FileName = "SeatingCharts",
                DefaultExt = ".xlsx",
                Filter = "Excel Sheet (*.xls, *.xlsx)|*.xls;*.xlsx"
            };
            var result = saveDialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    using var stream = saveDialog.OpenFile();
                    ExcelPackage.License.SetNonCommercialPersonal("Ryan Luttrull");
                    using (ExcelPackage package = GenerateExcelPackage(seatingCharts))
                    {
                        package.SaveAs(stream);
                    }
                    System.Windows.MessageBox.Show("File Created");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
    }
}