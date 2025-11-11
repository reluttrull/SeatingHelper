using System;
using System.Collections.Generic;
using System.Text;

namespace SeatingHelper.Model
{
    class Piece
    {
        public string Name { get; set; } = string.Empty;
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
