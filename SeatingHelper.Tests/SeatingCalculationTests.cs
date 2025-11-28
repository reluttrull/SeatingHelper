using SeatingHelper.Model;
using NUnit;
using NUnit.Framework;

namespace SeatingHelper.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestLeftBlock()
        {
            Piece simpleLeftBlock = new Piece()
            {
                Name = "Simple Left Block",
                Assignments = new List<Assignment>()
                {
                    new Assignment("Roger", "1"),
                    new Assignment("Nancy", "1"),
                    new Assignment("Malik", "2"),
                    new Assignment("Jenny", "2"),
                    new Assignment("Hans", "2"),
                    new Assignment("Aria", "3"),
                    new Assignment("Peter", "3"),
                    new Assignment("Valerie", "3")
                }
            };
            SeatingCalculator seatingCalculator = new SeatingCalculator(simpleLeftBlock, 2, 4);
            bool success = seatingCalculator.TryBlockPieceSeating(out Assignment[][] seating);
            Assert.That(success, Is.True);
            Assert.That(seating[0][0].PartName, Is.EqualTo(seating[1][0].PartName));
            Assert.That(seating[0].Length, Is.EqualTo(4));
            Assert.That(seating[1].Length, Is.EqualTo(4));
        }

        [Test]
        public void TestRightBlock()
        {
            Piece simpleLeftBlock = new Piece()
            {
                Name = "Simple Left Block",
                Assignments = new List<Assignment>()
                {
                    new Assignment("Roger", "1"),
                    new Assignment("Nancy", "1"),
                    new Assignment("Malik", "1"),
                    new Assignment("Jenny", "2"),
                    new Assignment("Hans", "2"),
                    new Assignment("Aria", "3"),
                    new Assignment("Peter", "3"),
                    new Assignment("Valerie", "3")
                }
            };
            SeatingCalculator seatingCalculator = new SeatingCalculator(simpleLeftBlock, 2, 4);
            bool success = seatingCalculator.TryBlockPieceSeating(out Assignment[][] seating);
            Assert.That(success, Is.True);
            Assert.That(seating[0].Length, Is.EqualTo(4));
            Assert.That(seating[1].Length, Is.EqualTo(4));
            Assert.That(seating[0][3].PartName, Is.EqualTo(seating[1][3].PartName));
        }

        [Test]
        public void TestStraight()
        {
            Piece simpleStraight = new Piece()
            {
                Name = "Simple Left Straight",
                Assignments = new List<Assignment>()
                {
                    new Assignment("Roger", "1"),
                    new Assignment("Nancy", "1"),
                    new Assignment("Fred", "1"),
                    new Assignment("Malik", "2"),
                    new Assignment("Jenny", "2"),
                    new Assignment("Hans", "3"),
                    new Assignment("Aria", "3"),
                    new Assignment("Peter", "3"),
                    new Assignment("Valerie", "3")
                }
            };
            SeatingCalculator seatingCalculator = new SeatingCalculator(simpleStraight, 2, 5);
            bool success = seatingCalculator.TryLongerRowsPieceSeating(out Assignment[][] seating);
            Assert.That(success, Is.True);
            Assert.That(seating[0].Length, Is.EqualTo(5));
            Assert.That(seating[1].Length, Is.EqualTo(4));
            Assert.That(seating[0][3].PartName, Is.EqualTo("2"));
            Assert.That(seating[1][0].PartName, Is.EqualTo("3"));
        }

        [Test]
        public void TestPriority()
        {
            Piece allSamePart = new Piece()
            {
                Name = "Testing full priority",
                Assignments = new List<Assignment>()
                {
                    new Assignment("Roger", "1", 2),
                    new Assignment("Nancy", "1", 1),
                    new Assignment("Fred", "1", 4),
                    new Assignment("Malik", "1", 3),
                    new Assignment("Jenny", "1", 5),
                    new Assignment("Hans", "1", 6)
                }
            };
            SeatingCalculator seatingCalculator = new SeatingCalculator(allSamePart, 1, 6);
            bool success = seatingCalculator.TryLongerRowsPieceSeating(out Assignment[][] seating);
            Assert.That(success, Is.True);
            Assert.That(seating[0].Length, Is.EqualTo(6));
            Assert.That(seating[0][0].PlayerName, Is.EqualTo("Nancy"));
            Assert.That(seating[0][1].PlayerName, Is.EqualTo("Roger"));
            Assert.That(seating[0][2].PlayerName, Is.EqualTo("Malik"));
            Assert.That(seating[0][3].PlayerName, Is.EqualTo("Fred"));
            Assert.That(seating[0][4].PlayerName, Is.EqualTo("Jenny"));
            Assert.That(seating[0][5].PlayerName, Is.EqualTo("Hans"));
        }

        [Test]
        public void TestPartialPriority()
        {
            Piece allSamePart = new Piece()
            {
                Name = "Testing partial priority",
                Assignments = new List<Assignment>()
                {
                    new Assignment("Roger", "1", 2),
                    new Assignment("Nancy", "1", 1),
                    new Assignment("Fred", "1"),
                    new Assignment("Malik", "1", 3),
                    new Assignment("Jenny", "1"),
                    new Assignment("Hans", "1")
                }
            };
            SeatingCalculator seatingCalculator = new SeatingCalculator(allSamePart, 1, 6);
            bool success = seatingCalculator.TryLongerRowsPieceSeating(out Assignment[][] seating);
            Assert.That(success, Is.True);
            Assert.That(seating[0].Length, Is.EqualTo(6));
            Assert.That(seating[0][0].PlayerName, Is.EqualTo("Nancy"));
            Assert.That(seating[0][1].PlayerName, Is.EqualTo("Roger"));
            Assert.That(seating[0][2].PlayerName, Is.EqualTo("Malik"));
        }

        [Test]
        public void TestScoreOrder()
        {
            Piece differentInstruments = new Piece()
            {
                Name = "All on different instruments",
                Assignments = new List<Assignment>()
                {
                    new Assignment("Roger", "Violin", 2),
                    new Assignment("Nancy", "Violin", 1),
                    new Assignment("Fred", "Flute"),
                    new Assignment("Malik", "Tuba"),
                    new Assignment("Jenny", "Flute", 1),
                    new Assignment("Hans", "Cello")
                }
            };
            SeatingCalculator seatingCalculator = new SeatingCalculator(differentInstruments, 1, 6);
            seatingCalculator.ScoreOrder = new List<string>() { "Flute", "Clarinet", "Trumpet", "Horn", "Trombone", "Tuba", "Violin", "Viola", "Cello" };
            bool success = seatingCalculator.TryLongerRowsPieceSeating(out Assignment[][] seating);
            Assert.That(success, Is.True);
            Assert.That(seating[0].Length, Is.EqualTo(6));
            Assert.That(seating[0][0].PlayerName, Is.EqualTo("Jenny"));
            Assert.That(seating[0][1].PlayerName, Is.EqualTo("Fred"));
            Assert.That(seating[0][2].PlayerName, Is.EqualTo("Malik"));
            Assert.That(seating[0][3].PlayerName, Is.EqualTo("Nancy"));
            Assert.That(seating[0][5].PlayerName, Is.EqualTo("Hans"));
        }
    }
}
