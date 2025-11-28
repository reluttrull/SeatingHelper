using System;
using System.Collections.Generic;
using System.Text;

namespace SeatingHelper.Model
{
    public class ChartListViewItem
    {
        public int Rows { get; set; }
        public long Players { get; set; }
        public Assignment[][] Chart { get; set; }

        public ChartListViewItem(Assignment[][] chart)
        {
            if (chart is null) chart = [];
            Chart = new Assignment[chart.Length][];
            for (int i = 0; i < chart.Length; i++)
            {
                Chart[i] = new Assignment[chart[i].Length];
                Array.Copy(chart[i], Chart[i], chart[i].Length);
            }
            Rows = chart.Length;
            Players = chart.Sum(row => row.Length);
        }
    }
}
