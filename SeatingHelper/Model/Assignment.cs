using System;
using System.Collections.Generic;
using System.Text;

namespace SeatingHelper.Model
{
    public class Assignment : IComparable<Assignment>
    {
        public string PlayerName { get; set; } = string.Empty;
        public string PartName {  get; set; } = string.Empty;
        public int Priority { get; set; } = Int32.MaxValue;
        public Assignment(string playerName, string partName)
        {
            PlayerName = playerName;
            PartName = partName;
        }
        public Assignment(string playerName, string partName, int priority)
        {
            PlayerName = playerName;
            PartName = partName;
            Priority = priority;
        }
        public int CompareTo(Assignment? other)
        {
            if (other == null) return 1;
            int partComparison = this.PartName.CompareTo(other.PartName);
            if (partComparison != 0) return partComparison;
            int priorityComparison = this.Priority.CompareTo(other.Priority);
            if (priorityComparison != 0) return priorityComparison;

            return this.PlayerName.CompareTo(other.PlayerName);
        }
    }
}
