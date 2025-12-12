using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SeatingHelper.Model
{
    public class ChartListViewItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private int _rows;
        public int Rows
        {
            get { return _rows; }
            set
            {
                if (_rows != value)
                {
                    _rows = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _players;
        public int Players
        {
            get { return _players; }
            set
            {
                if (_players != value)
                {
                    _players = value;
                    OnPropertyChanged();
                }
            }
        }
        public Assignment[][] Chart { get; set; }

        public ChartListViewItem(Assignment[][] chart, string name)
        {
            Name = name;
            if (chart is null) chart = [];
            Chart = new Assignment[chart.Length][];
            for (int i = 0; i < chart.Length; i++)
            {
                Chart[i] = new Assignment[chart[i].Length];
                Array.Copy(chart[i], Chart[i], chart[i].Length);
            }
            Rows = Chart.Length;
            Players = Chart.Sum(row => row.Length);
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
