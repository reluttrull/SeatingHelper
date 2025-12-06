using SeatingHelper.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ChartDisplayWindow.xaml
    /// </summary>
    public partial class ChartDisplayWindow : Window
    {
        public ObservableCollection<DisplayRow> SeatingChart { get; set; }
        public ChartDisplayWindow(Assignment[][] seating)
        {
            InitializeComponent();
            SeatingChart = new();
            for (int i = 0; i < seating.Length; i++)
            {
                DisplayRow displayRow = new DisplayRow(seating[i]);
                displayRow.RowNumber = i + 1;
                SeatingChart.Add(displayRow);
            }
            DataContext = this;
        }
    }
    public class DisplayRow
    {
        public int RowNumber { get; set; }
        public ObservableCollection<Assignment> InnerList { get; set; }
        public DisplayRow(Assignment[] row)
        {
            InnerList = new ObservableCollection<Assignment>(row);
        }
    }
}
