using System;
using Domain;
using NUnit.Framework;

namespace Tests
{
    public class TestBoard
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateDefaultBoard()
        {
            var board = new Board();
            Assert.AreEqual(10, board.GameBoard.Capacity);
            foreach (var col in board.GameBoard)
            {
                Assert.AreEqual(10, col.Capacity);
            }
        }

        [Test]
        public void CreateCustomBoard()
        {
            var customBoard = new Board(totalRows: 21, totalCols: 13, canTouch: false);
            Assert.AreEqual(expected: 21, actual: customBoard.GameBoard.Capacity);
            foreach (var col in customBoard.GameBoard)
            {
                Assert.AreEqual(expected: 13, actual: col.Capacity);
            }
        }

        [Test]
        public void TestAddBattleshipNormal()
        {
            var board = new Board();
            var battleship = new Battleship(3);
            board.AddBattleship((0, 0), (0, 2), battleship);
            Assert.AreSame(battleship, board.GetData(0,0));
            Assert.AreSame(battleship, board.GetData(0,2));
        }

        [Test]
        public void TestAddBattleshipDiagonal()
        {
            var board = new Board();
            var battleship = new Battleship(2);
            Assert.That(() => board.AddBattleship((0,0),(1,1),battleship), Throws.ArgumentException);
        }

        [Test]
        public void TestAddBattleshipOnAnotherShip()
        {
            var board = new Board();
            Battleship ship1 = new Battleship(1), ship2 = new Battleship(3);
            board.AddBattleship((0,0),(0,2),ship2);
            Assert.That(() => board.AddBattleship((0,1),(0,1),ship1), Throws.ArgumentException);
        }

        [Test]
        public void TestAddBattleshipOutOfBounds()
        {
            var board = new Board();
            var ship = new Battleship(1);
            Assert.That(() => board.AddBattleship((10,10),(10,10),ship), Throws.ArgumentException);
        }

        [Test]
        public void TestAddBattleshipPointsDistanceNotSameLengthAsShip()
        {
            var board = new Board();
            Battleship ship1 = new Battleship(3), ship2 = new Battleship(1);
            
            Assert.That(() => board.AddBattleship((0,0),(0,1),ship1), Throws.ArgumentException);
            Assert.That(() => board.AddBattleship((0,0),(0,1),ship2), Throws.ArgumentException);
        }

        [Test]
        public void TestGetAllCoords()
        {
            Console.WriteLine("Test Horizontal");
            
            var list = Board.GetAllCoords((0, 0), (0, 5));
            for (var i = 0; i < 6; i++)
            {
                Console.WriteLine($"{list[i].Item1} : {list[i].Item2}");
                Assert.That(list[i].row == 0);
                Assert.That(list[i].col == i);
            }

            Console.WriteLine("Test Vertical");
            
            var list2 = Board.GetAllCoords((0, 0), (5, 0));
            for (var i = 0; i < 6; i++)
            {
                Console.WriteLine($"{list[i].Item1} : {list[i].Item2}");
                Assert.That(list2[i].row == i);
                Assert.That(list2[i].col == 0);
            }
        }
        
        [Test]
        public void TestGetAllCoordsReverse()
        {
            Console.WriteLine("Test Horizontal");
            
            var list = Board.GetAllCoords((0, 5), (0, 0));
            for (var i = 0; i < 6; i++)
            {
                Console.WriteLine($"{list[i].Item1} : {list[i].Item2}");
                Assert.That(list[i].row == 0);
                Assert.That(list[i].col == i);
            }

            Console.WriteLine("Test Vertical");
            
            var list2 = Board.GetAllCoords((5, 0), (0, 0));
            for (var i = 0; i < 6; i++)
            {
                Console.WriteLine($"{list[i].Item1} : {list[i].Item2}");
                Assert.That(list2[i].row == i);
                Assert.That(list2[i].col == 0);
            }
        }

        [Test]
        public void TestGetData()
        {
            var board = new Board();
            Assert.IsNull(board.GetData(0,0));
        }

        [Test]
        public void TestBombLocationContainingShip()
        {
            var board = new Board();
            var ship = new Battleship(3);
            board.AddBattleship((0,0), (0,2), ship);
            Assert.True(board.BombLocation(0,0));
            Assert.True(board.BombLocation(0,1));
            Assert.True(board.BombLocation(0,2));
        }

        [Test]
        public void TestBombLocationEmptyTile()
        {
            var board = new Board();
            Assert.False(board.BombLocation(0,0));
        }

        [Test]
        public void TestBombLocationTwice()
        {
            var board = new Board();
            var battleship = new Battleship(3);
            board.AddBattleship((0,0), (2,0), battleship);
            
            //Bomb empty tile
            Assert.False(board.BombLocation(1,1));
            Assert.That(() => board.BombLocation(1,1), Throws.ArgumentException);
            
            //Bomb ship
            Assert.True(board.BombLocation(1,0));
            Assert.That(() => board.BombLocation(1,0), Throws.ArgumentException);
        }

        [Test]
        public void TestBombings()
        {
            var board = new Board();
            var battleship = new Battleship(1);
            
            board.AddBattleship((0,0), (0,0), battleship);
            board.BombLocation(0, 0);
            board.BombLocation(1, 0);
            board.BombLocation(2, 0);
            
            Assert.AreSame(board.Bombings[0].Item1, board.GameBoard[0][0]);
            Assert.AreSame(board.Bombings[1].Item1, board.GameBoard[1][0]);
            Assert.AreSame(board.Bombings[2].Item1, board.GameBoard[2][0]);
            
            Assert.AreEqual(board.Bombings[0].Item2, BombingResult.Hit);
            Assert.AreEqual(board.Bombings[1].Item2, BombingResult.Miss);
            Assert.AreEqual(board.Bombings[2].Item2, BombingResult.Miss);
        }

        [Test]
        public void TestGetBattleshipLocations()
        {
            var board = new Board();
            Battleship ship1 = new Battleship(2), ship2 = new Battleship(2);
            board.AddBattleship((0,0), (0,1), ship1);
            board.AddBattleship((1,0), (1,1), ship2);
            var locations = board.GetBattleshipLocations();
            Assert.True(locations.Count == 4);
            Assert.True(locations.Contains(board.GameBoard[0][0]));
            Assert.True(locations.Contains(board.GameBoard[0][1]));
            Assert.True(locations.Contains(board.GameBoard[1][0]));
            Assert.True(locations.Contains(board.GameBoard[1][1]));
        }
    }
}