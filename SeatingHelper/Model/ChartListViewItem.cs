using System;
using System.Collections.Generic;
using System.Text;

namespace SeatingHelper.Model
{
    public class ChartListViewItem
    {
        public int Rows { get; }
        public long Players { get; }
        public Assignment[][] Chart { get; }

        public ChartListViewItem(Assignment[][] chart)
        {
            Chart = chart;
            Rows = chart.Length;
            Players = chart.Sum(row => row.Length);
        }
    }
}
