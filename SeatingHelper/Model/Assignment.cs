using System;
using System.Collections.Generic;
using System.Text;

namespace SeatingHelper.Model
{
    public class Assignment : IComparable<Assignment>
    {
        public string PlayerName { get; set; } = string.Empty;
        public string PartName {  get; set; } = string.Empty;
        public Assignment(string playerName, string partName)
        {
            PlayerName = playerName;
            PartName = partName;
        }
        public int CompareTo(Assignment? other)
        {
            if (other == null) return 1;
            int partComparison = this.PartName.CompareTo(other.PartName);
            if (partComparison != 0) return partComparison;

            return this.PlayerName.CompareTo(other.PlayerName);
        }
    }
}
