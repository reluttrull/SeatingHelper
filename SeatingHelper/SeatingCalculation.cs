using SeatingHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool TryLongerRowsPieceSeating(Piece piece, int rows, out Assignment[][] seating)
        {
            seating = new Assignment[rows][];
            var groups = piece.Assignments.OrderBy(a => a.PartName).GroupBy(a => a.PartName).Select(g => new List<IGrouping<string,Assignment>>() { g }).ToList();
            int extraRows = groups.Count - rows;
            if (extraRows == 0)
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    seating[i] = groups[i].SelectMany(g => g).ToArray();
                }
            }
            int step = 0;
            while (groups.Count > rows)
            {
                Console.WriteLine(step % (groups.Count - 1) + 1);
                for (int i = (step % (groups.Count - 1) + 1); i < groups.Count; i++)
                {
                    IGrouping<string, Assignment> group = groups[i].First();
                    groups[i - 1].Add(group);
                    groups[i].Remove(group);
                }
                step++;
                if (groups[groups.Count - 1].Count == 0) groups.RemoveAt(groups.Count - 1);
                if (step > 100) return false;
            }
            for (int i = 0; i < groups.Count; i++)
            {
                seating[i] = groups[i].SelectMany(g => g).ToArray();
            }
            return true;
        }
    }
}
