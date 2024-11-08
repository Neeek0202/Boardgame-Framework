using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization; 

namespace BoardgameFramework
{
    [Serializable] // Marks the class as serializable and includes AIPlayer in the XML serialization process
    [XmlInclude(typeof(AIPlayer))]
    public class AIPlayer : Player
    {
        private Random random;
        private HashSet<int> usedNumbers; //Tracks numbers that has been used 

        public AIPlayer() : base() { } //default constructor 

        public AIPlayer(char symbol) : base(symbol) //constructor to initialize AI player with specific symbol 
        {
            random = new Random();
            usedNumbers = new HashSet<int>();
        }

        public void InitializeLoadAIPlayer() // Method to re-initialize fields when loading a game state. This is necessary because the random generator and usedNumbers are not serialized and need to be reinitialized
        {
            random = new Random();
            usedNumbers = new HashSet<int>();
        }


        public override (int row, int col) GetMove(Board board, GameRules gameRules) //Method for to get generating random valid move  
        {
            int row, col;
            do
            {
                row = random.Next(0, board.width);
                col = random.Next(0, board.height);
            } while (board[row, col] != '.'); // Ensure the cell is empty

            Console.WriteLine($"AI ({Symbol}) chooses: Row {row + 1}, Column {col + 1}");
            return (row, col);
        }

        public int GetValidNumber()
        {
            List<int> availableNumbers = Enumerable.Range(1, 9)
                                                   .Where(n => n % 2 == 0 && !usedNumbers.Contains(n)) //Generates even numbers between range 1-9 
                                                   .ToList();
            int number = availableNumbers[random.Next(availableNumbers.Count)];
            usedNumbers.Add(number); //Marks the selected number as used to avoid repeats 
            return number;
        }
    }
}