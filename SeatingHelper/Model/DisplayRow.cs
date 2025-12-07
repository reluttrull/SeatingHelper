using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SeatingHelper.Model
{
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
