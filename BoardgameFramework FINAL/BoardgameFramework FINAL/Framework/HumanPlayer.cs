using System;
using System.Xml.Serialization;

namespace BoardgameFramework
{
    [Serializable] // Marks the class as serializable and includes HumanPlayer in the XML serialization process
    [XmlInclude(typeof(HumanPlayer))]
    public class HumanPlayer : Player
    {
        public HumanPlayer() : base() { }
        public HumanPlayer(char symbol) : base(symbol) { }    // Constructor to initialize the HumanPlayer with a specific symbol

        public override (int row, int col) GetMove(Board board, GameRules gameRules)
        {
            while (true)
            {
                Console.WriteLine("Enter 's' to save, 'u' to undo, 'r' to redo, 'h' for help, 'm' to make a move:");
                string input = Console.ReadLine()?.ToLower() ?? string.Empty;

                if (input == "s")
                {
                    return (-3, -3); // Since GetMove() returns a tuple, Special values are created to correspond to each function - save 
                }
                else if (input == "u")
                {
                    return (-1, -1); // Special value to indicate undo
                }
                else if (input == "r")
                {
                    return (-2, -2); // Special value to indicate redo
                }
                else if (input == "h")
                {
                    gameRules.DisplayRules();
                    continue; // Return to the game loop after displaying rules
                }
                else if (input == "m")
                {
                    int row = GetValidInput("Row", 1, board.width);
                    int col = GetValidInput("Column", 1, board.height);
                    return (row - 1, col - 1); // Convert to 0-based index
                }
                else
                {

                    Console.WriteLine("Invalid input. Please enter 's', 'u', 'r', 'h', 'm'.");

                }
            }
        }

        private int GetValidInput(string prompt, int minValue, int maxValue)  // Ensures that the input is within the valid range 
        {
            int value;
            while (true)
            {
                Console.Write($"{prompt} ({minValue}-{maxValue}): ");
                if (int.TryParse(Console.ReadLine(), out value) && value >= minValue && value <= maxValue)
                {
                    return value;
                }
                else
                {
                    Console.WriteLine($"Invalid input. Please enter a number between {minValue} and {maxValue}.");
                }
            }
        }
    }
}