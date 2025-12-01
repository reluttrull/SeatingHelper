using SeatingHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeatingHelper
{
    public class SeatingCalculator
    {
        public Piece Piece {  get; set; }
        public int Rows { get; set; }
        public int MaxRowWidth { get; set; }
        public List<string>? ScoreOrder { get; set; } = null;
        public SeatingCalculator(Piece piece, int rows, int maxRowWidth) 
        {
            Piece = piece;
            Rows = rows;
            MaxRowWidth = maxRowWidth;
        }
        public bool TryLongerRowsPieceSeating(out Assignment[][] seating)
        {
            seating = new Assignment[Rows][];
            List<Assignment[]> temporarySeating = [];
            List<IGrouping<string, Assignment>> groups = new();
            if (ScoreOrder is not null) groups = Piece.Assignments
                    .OrderBy(a => a.Priority)
                    .GroupBy(a => a.PartName)
                    .OrderBy(g => ScoreOrder.IndexOf(g.Key))
                    .ToList();
            else groups = Piece.Assignments
                    .OrderBy(a => a.Priority)
                    .GroupBy(a => a.PartName)
                    .OrderBy(g => g.Key.Length)
                    .ThenBy(g => g.Key)
                    .ToList();

            int row = 0;
            while (groups.Count > 0)
            {
                int testGroupSum = 0;
                var testGroups = groups.TakeWhile(g =>
                {
                    int groupCount = g.Count();
                    if (testGroupSum + groupCount > MaxRowWidth) return false;
                    testGroupSum += groupCount;
                    return true;
                }).ToList();
                temporarySeating.Add([.. testGroups.SelectMany(g => g)]);
                groups.RemoveAll(g => testGroups.Any(tg => tg.Key == g.Key));
                row++;
            }
            seating = [.. temporarySeating];

            if (seating.Length == 0 || seating.Length > Rows) return false;
            return true;
        }

        public bool TryBlockPieceSeating(out Assignment[][] seating)
        {
            seating = new Assignment[Rows][];
            int smallestRowWidth = (int)Math.Ceiling((double)Piece.Assignments.Count / Rows);
            List<IGrouping<string, Assignment>> groups = new();
            if (ScoreOrder is not null) groups = Piece.Assignments
                    .OrderBy(a => a.Priority)
                    .GroupBy(a => a.PartName)
                    .OrderBy(g => ScoreOrder.IndexOf(g.Key))
                    .ToList();
            else groups = Piece.Assignments
                    .OrderBy(a => a.Priority)
                    .GroupBy(a => a.PartName)
                    .OrderBy(g => g.Key.Length)
                    .ThenBy(g => g.Key)
                    .ToList();

            List<Assignment[]> temporarySeating = [];
            while (groups.Count > 0)
            {
                int testGroupSum = 0;
                var testGroup = groups.TakeWhile(g =>
                {
                    int groupCount = g.Count();
                    if (testGroupSum + groupCount > (MaxRowWidth * 2)) return false;
                    testGroupSum += groupCount;
                    return true;
                }).ToList();
                int testGroupRowWidth = (int)Math.Ceiling((double)testGroupSum / 2);
                var testGroupAssignments = testGroup.SelectMany(g => g).ToList();
                var leftmostAssignments = testGroup.First().ToList(); // leftmost group assignments
                var rightmostAssignments = testGroupAssignments.Count < MaxRowWidth ? [] : testGroup.Where(g => g.Key == testGroupAssignments[MaxRowWidth - 1].PartName).First().ToList(); // rightmost assignments
                // if two rows already fit perfectly
                if (testGroupAssignments.Count >= testGroupRowWidth + 1 
                    && testGroupAssignments[testGroupRowWidth - 1].PartName != testGroupAssignments[testGroupRowWidth].PartName
                    && testGroupSum > smallestRowWidth) 
                {
                    int frontRowSum = 0;
                    Assignment[] frontRow = testGroup.TakeWhile(g =>
                    {
                        frontRowSum += g.Count();
                        return frontRowSum <= testGroupRowWidth;
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
                    Piece remainingAssignments = new Piece() { Assignments = testGroup.Where(g => g.Key != leftmostAssignments.First().PartName).SelectMany(g => g).ToList() };
                    SeatingCalculator recursiveCalculator = new SeatingCalculator(remainingAssignments, 2, testGroupRowWidth - (leftmostAssignments.Count / 2));
                    bool recursionSuccess = recursiveCalculator.TryBlockPieceSeating(out Assignment[][] rowRemainders);

                    if (!recursionSuccess) return false; // for now, just break

                    Array.Copy(rowRemainders[0], 0, frontRow, leftmostCol, rowRemainders[0].Length);
                    Array.Copy(rowRemainders[1], 0, backRow, leftmostCol, rowRemainders[1].Length);

                    temporarySeating.Add([..frontRow.Where(item => item != null)]);
                    temporarySeating.Add([..backRow.Where(item => item != null)]);
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

                    Piece remainingAssignments = new Piece() { Assignments = testGroup.Where(g => g.Key != rightmostAssignments.First().PartName).SelectMany(g => g).ToList() };
                    SeatingCalculator recursiveCalculator = new SeatingCalculator(remainingAssignments, 2, testGroupRowWidth - (rightmostAssignments.Count / 2));
                    bool recursionSuccess = recursiveCalculator.TryBlockPieceSeating(out Assignment[][] rowRemainders);

                    if (!recursionSuccess) return false; // for now, just break
                    
                    Array.Copy(rowRemainders[0], 0, frontRow, 0, rowRemainders[0].Length);
                    Array.Copy(rowRemainders[1], 0, backRow, 0, rowRemainders[1].Length);

                    temporarySeating.Add([..frontRow.Where(item => item != null)]);
                    temporarySeating.Add([..backRow.Where(item => item != null)]);
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
                    if (frontRow.Length == 0) return false; // we couldn't fit into the line
                    temporarySeating.Add(frontRow);
                    groups.RemoveAll(g => frontRow.Any(a => a.PartName == g.Key));
                }
            }
            seating = [..temporarySeating];

            if (seating.Length == 0 || seating.Length > Rows) return false;
            return true;
        }

        public Assignment[][] CondenseRows(Assignment[][] blockSeating)
        {
            int currentRow = Rows - 1;
            List<Assignment> assignmentsToMove = blockSeating.Where((row, index) => index >= Rows).SelectMany(row => row).Reverse().ToList();
            for (int i = 0; i < Rows; i++)
            {
                Array.Resize(ref blockSeating[i], MaxRowWidth);
            }
            foreach (Assignment assignmentToMove in assignmentsToMove)
            {
                int index = -1;
                while (index < 0 && currentRow >= 0)
                {
                    index = GetLastOpenSeat(blockSeating[currentRow]);
                    if (index < 0) currentRow--;
                }
                if (currentRow < 0) break;
                blockSeating[currentRow][index] = assignmentToMove;
            }
            Array.Resize(ref blockSeating, Rows);
            return [..blockSeating.Select(row => row.Where(item => item != null).ToArray())];
        }

        private int GetLastOpenSeat(Assignment[] row)
        {
            for (int i = row.Length - 1; i >= 0; i--)
            {
                if (row[i] is null) return i;
            }
            return -1;
        }
    }
}
