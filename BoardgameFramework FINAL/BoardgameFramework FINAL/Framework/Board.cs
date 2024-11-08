using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BoardgameFramework
{
    [Serializable]
    public class Board
    {
        public int width;
        public int height;
        [XmlIgnore] //ignored during serialization 
        public char[,] grid; //The 2D array representing the board's cells.

        [XmlArray("Grid")]
        [XmlArrayItem("Row")]
        public List<List<char>> Grid { get; set; } //A List of Lists that represents the grid for serialization purposes.

        public Board() // Default constructor for serialization
        {
            Grid = new List<List<char>>();
        }

        public Board(int width, int height)  // Constructor to initialize the board with given dimensions depending on size of board of a game 
        {
            this.width = width;
            this.height = height;
            grid = new char[width, height];
            Grid = new List<List<char>>();
            InitializeBoard();
        }

        private void InitializeBoard() //Initializes empty board 
        {
            for (int x = 0; x < width; x++)
            {
                Grid.Add(new List<char>(new char[height])); //Adds a new row to the Grid list.
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = '.';
                    Grid[x][y] = '.';
                }
            }
            DisplayBoard();
        }

        public void PlacePiece(char symbol, int x, int y) // Method to place a piece on the board
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (grid[y, x] == '.' || symbol == '.')
                {
                    grid[y, x] = symbol;
                    Grid[x][y] = symbol;
                }
                else
                {
                    throw new InvalidOperationException("Cell is already occupied.");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Position is out of the board's bounds.");
            }
        }

        public void DisplayBoard() // Method to display the current state of the board using a nested for loop
        {
            Console.Write("  ");
            for (int x = 0; x < width; x++)
            {
                Console.Write($"{x + 1} ");
            }
            Console.WriteLine();

            for (int y = 0; y < height; y++)
            {
                Console.Write($"{y + 1} ");
                for (int x = 0; x < width; x++)
                {
                    Console.Write(grid[x, y] + " ");
                }
                Console.WriteLine();
            }
        }

        public char this[int x, int y] // Indexer to access the board's cells using 0-based indexing
        {
            get { return grid[x, y]; } //Provides direct access to the in-memory grid using array-like indexing
        }
    }
}