using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Board
    {
        public List<List<Tile>> Tiles { get; set; }
        private List<Battleship> Battleships { get; set; } = new List<Battleship>();
        public List<(Tile, BombingResult)> Bombings { get; set; } = new List<(Tile, BombingResult)>();
        private bool CanTouch { get; set; }
        private int MaxCol, MaxRow;

        public Board(int totalRows = 10, int totalCols = 10, bool canTouch = true)
        {
            MaxRow = totalRows - 1;
            MaxCol = totalCols - 1;
            CanTouch = canTouch;
            Tiles = new List<List<Tile>>(totalRows);
            for (var row = 0; row < totalRows; row++)
            {
                Tiles.Add(new List<Tile>(totalCols));
                for (var col = 0; col < totalCols; col++)
                {
                    Tiles[row].Add(new Tile(row, col, this));
                }
            }
        }

        public void AddBattleship((int row, int col) start, (int row, int col) end, Battleship battleship)
        {
            //Check that ship end points are on the same axis
            if (!(start.row == end.row || start.col == end.col))
            {
                throw new ArgumentException("Start and end pieces are not are not on the same axis!");
            }

            //Check that points are inside the board
            if (start.row < 0 || end.row < 0 || start.col < 0 || end.col < 0 ||
                start.row > MaxRow || end.row > MaxRow || start.col > MaxCol || end.col > MaxCol)
            {
                throw new ArgumentException("One of the points is not inside the board!");
            }

            //Check that ship size is correct

            if (start.row == end.row && Math.Abs(start.col - end.col) + 1 != battleship.Size ||
                start.col == end.col && Math.Abs(start.row - end.row) + 1 != battleship.Size)
            {
                Console.WriteLine(Math.Abs(start.col - end.col) + 1);
                Console.WriteLine(battleship.Size);
                Console.WriteLine($"({start.row}:{start.col})({end.row}:{end.col})");
                throw new ArgumentException("Ship is longer than distance between points!");
            }

            //Check, that ship is not on other ships

            foreach (var coord in GetAllCoords((start.row, start.col), (end.row, end.col)))
            {
                if (!Tiles[coord.row][coord.col].IsEmpty())
                {
                    throw new ArgumentException($"({coord.row} : {coord.col}) already contains a ship!");
                }
            }

            //Check that ship is not too close to other ships
            if (!CanTouch)
            {
                var neighbourTiles = new List<Tile>();
                foreach (var coord in GetAllCoords((start.row, start.col), (end.row, end.col)))
                {
                    if (coord.col + 1 <= MaxCol) neighbourTiles.Add(Tiles[coord.row][coord.col + 1]);
                    if (coord.col - 1 <= 0)      neighbourTiles.Add(Tiles[coord.row][coord.col - 1]);
                    if (coord.row + 1 <= MaxRow) neighbourTiles.Add(Tiles[coord.row + 1][coord.col]);
                    if (coord.row - 1 <= 0)      neighbourTiles.Add(Tiles[coord.row - 1][coord.col]);
                }

                foreach (var tile in neighbourTiles)
                {
                    if (!tile.IsEmpty() && tile.Battleship != battleship)
                    {
                        throw new ArgumentException($"Another ship is touching current ship at ({tile.Row} : {tile.Col})");
                    }
                }
            }

            Battleships.Add(battleship);
            battleship.RelatedBoard = this;
            
            foreach (var coord in GetAllCoords(start:(start.row, start.col), end: (end.row, end.col)))
            {
                var tile = Tiles[coord.row][coord.col];
                tile.Battleship = battleship;
                battleship.Locations.Add(tile);
            }

        }        
        
        public List<Tile> GetBattleshipLocations()
        {
            var result = new List<Tile>();
            
            foreach (var battleship in Battleships)
            {
                foreach (var tile in battleship.Locations)
                {
                    result.Add(tile);
                }
            }

            return result;
        }
            
        public Battleship GetData(int row, int col)
        {
            if (row > Tiles.Count - 1 || col > Tiles[0].Count)
            {
                throw new ArgumentException("Out of range ");
            }
            return Tiles[row][col]?.Battleship;
        }
        
        public bool BombLocation(int row, int col)
        {
            if (row > Tiles.Count - 1 || col > Tiles[0].Count)
            {
                throw new ArgumentException("Out of range ");
            }    
            var targetTile = Tiles[row][col];
            
            if (targetTile.IsBombed) throw new ArgumentException("This tile has been already bombed.");

            targetTile.IsBombed = true;
            if (targetTile.IsEmpty())
            {
                Bombings.Add((targetTile, BombingResult.Miss));
                return false;
            }

            Bombings.Add((targetTile, BombingResult.Hit));
            targetTile.Battleship.LivesLeft--;            
            return true;
        }

        public static List<(int row, int col)> GetAllCoords((int row, int col) start, (int row, int col) end)
        {
            int loopStart, loopEnd;
            bool isVertical = end.col == start.col;
            
            if (isVertical)
            {
                loopStart = end.row > start.row ? start.row : end.row;
                loopEnd = end.row > start.row ? end.row : start.row;
            }
            else
            {
                loopStart = end.col > start.col ? start.col: end.col;
                loopEnd = end.col > start.col ? end.col: start.col;
            }
            var result = new List<(int, int)>();
            for (var i = loopStart; i <= loopEnd; i++)
            {
                result.Add(isVertical ? (row: i, col: start.col) : (row: start.row, col: i));
            }

            return result;

        }

        public bool AnyShipsLeft()
        {
            foreach (var battleship in Battleships)
            {
                if (battleship.IsAlive()) return true;
            }
            return false;
        }
    }
}