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

        public static bool TryBlockPieceSeating(Piece piece, int rows, int maxRowWidth, out Assignment[][] seating)
        {
            seating = new Assignment[rows][];
            int smallestRowWidth = (int)Math.Ceiling((double)piece.Assignments.Count / rows);
            var groups = piece.Assignments.GroupBy(a => a.PartName).OrderBy(g => g.Key).ToList();

            List<Assignment[]> temporarySeating = [];
            while (groups.Count > 0)
            {
                int testGroupSum = 0;
                var testGroup = groups.TakeWhile(g =>
                {
                    int groupCount = g.Count();
                    if (testGroupSum + groupCount > (maxRowWidth * 2)) return false;
                    testGroupSum += groupCount;
                    return true;
                }).ToList();
                int testGroupRowWidth = (int)Math.Ceiling((double)testGroupSum / 2);
                var testGroupAssignments = testGroup.SelectMany(g => g).ToList();
                var leftmostAssignments = testGroup.First().ToList(); // leftmost group assignments
                var rightmostAssignments = testGroupAssignments.Count < smallestRowWidth ? [] : testGroup.Where(g => g.Key == testGroupAssignments[smallestRowWidth - 1].PartName).First().ToList(); // rightmost assignments
                // if two rows already fit perfectly
                if (testGroupAssignments.Count >= smallestRowWidth + 1 && testGroupAssignments[smallestRowWidth - 1].PartName != testGroupAssignments[smallestRowWidth].PartName) 
                {
                    int frontRowSum = 0;
                    Assignment[] frontRow = testGroup.TakeWhile(g =>
                    {
                        frontRowSum += g.Count();
                        return frontRowSum <= smallestRowWidth;
                    }).SelectMany(g => g).ToArray();
                    Assignment[] backRow = [..testGroup.SelectMany(g => g).Except(frontRow)];
                    temporarySeating.Add(frontRow);
                    temporarySeating.Add(backRow);
                    groups.RemoveAll(g => testGroup.Any(tg => tg.Key == g.Key));
                }
                // if blocking and  leftmost is even
                else if (testGroupRowWidth >= smallestRowWidth && leftmostAssignments.Count % 2 == 0) 
                {
                    // use leftmost group as block
                    Assignment[] frontRow = new Assignment[testGroupRowWidth];
                    Assignment[] backRow = new Assignment[testGroupRowWidth];
                    int i;
                    for (i = 0; i < leftmostAssignments.Count; i++)
                    {
                        // fill block top -> bottom, left -> right
                        if (i % 2 == 0) frontRow[i / 2] = leftmostAssignments[i];
                        else backRow[i / 2] = leftmostAssignments[i];
                    }
                    int leftmostCol = i / 2; // column pointer
                    int index = 0;
                    Piece remainingAssignments = new Piece() { Assignments = testGroup.Where(g => g.Key != leftmostAssignments.First().PartName).SelectMany(g => g).ToList() };
                    TryBlockPieceSeating(remainingAssignments, 2, testGroupRowWidth - (leftmostAssignments.Count / 2), out Assignment[][] rowRemainders);
                    Array.Copy(rowRemainders[0], 0, frontRow, leftmostCol, rowRemainders[0].Length);
                    Array.Copy(rowRemainders[1], 0, backRow, leftmostCol, rowRemainders[0].Length);
                    temporarySeating.Add(frontRow);
                    temporarySeating.Add(backRow);
                    groups.RemoveAll(g => testGroup.Any(tg => tg.Key == g.Key));
                } 
                // if blocking and rightmost is even
                else if (testGroupRowWidth >= smallestRowWidth && rightmostAssignments.Count % 2 == 0)
                {
                    // try use group that overflows on right as block
                    Assignment[] frontRow = new Assignment[testGroupRowWidth];
                    Assignment[] backRow = new Assignment[testGroupRowWidth];
                    int i;
                    for (i = 0; i < rightmostAssignments.Count; i++)
                    {
                        // fill block top -> bottom, right -> left
                        int setIndex = testGroupRowWidth - (i / 2) - 1;
                        if (i % 2 == 0) frontRow[setIndex] = rightmostAssignments[i];
                        else backRow[setIndex] = rightmostAssignments[i];
                    }
                    int rightmostCol = testGroupRowWidth - (i / 2) - 1; // column pointer
                    int index = 0;
                    foreach (Assignment assignment in testGroup.Where(g => g.Key != rightmostAssignments.First().PartName).SelectMany(g => g))
                    {
                        // fill out the other groups starting at 0 and continuing up to pointer
                        if (index <= rightmostCol) frontRow[index] = assignment;
                        else backRow[index % (rightmostCol + 1)] = assignment;
                        index++;
                    }
                    temporarySeating.Add(frontRow);
                    temporarySeating.Add(backRow);
                    groups.RemoveAll(g => testGroup.Any(tg => tg.Key == g.Key));
                }
                // if cann't block, fill a single straight row as much as possible
                else
                {
                    int frontRowSum = 0;
                    Assignment[] frontRow = testGroup.TakeWhile(g =>
                    {
                        frontRowSum += g.Count();
                        return frontRowSum <= smallestRowWidth;
                    }).SelectMany(g => g).ToArray();
                    temporarySeating.Add(frontRow);
                    groups.RemoveAll(g => frontRow.Any(a => a.PartName == g.Key));
                }
            }
            seating = [..temporarySeating];

            return true;
        }
    }
}
