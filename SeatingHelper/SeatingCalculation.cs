using SeatingHelper.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeatingHelper
{
    public class SeatingCalculation
    {

        public static bool TrySimplePieceSeating(Piece piece, int rows, out Assignment[][] seating)
        {
            int minRowWidth = (int)Math.Ceiling((double)piece.Assignments.Count / rows);
            seating = new Assignment[rows][];
            for (int i = 0; i < seating.Length; i++)
            {
                seating[i] = new Assignment[minRowWidth];
            }
            int row = 0, seat = 0;
            var groups = piece.Assignments.GroupBy(a => a.PartName).OrderBy(g => g.Key);
            foreach (var group in groups)
            {
                if (group.Count() > (minRowWidth - seat)) return false;
                foreach (var assignment in group)
                {
                    Console.WriteLine($"setting seat [{row}][{seat}] to {assignment.PlayerName}");
                    seating[row][seat] = assignment;
                    seat++;
                }
                if (seat == minRowWidth)
                {
                    row++;
                    seat = 0;
                }
            }
            return true;
        }
    }
}
