using System;
using System.Collections.Generic;
using System.Text;

namespace SeatingHelper.Model
{
    public class Assignment
    {
        public string PlayerName { get; set; } = string.Empty;
        public string PartName {  get; set; } = string.Empty;
        public Assignment(string playerName, string partName)
        {
            PlayerName = playerName;
            PartName = partName;
        }
    }
}
