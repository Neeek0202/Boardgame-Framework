using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace BoardgameFramework 
{
    public class GameState
    {
        // Properties to store game state information in XML file 
        public string Name { get; set; }
        public Board Board { get; set; }
        public Player CurrentPlayer { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public List<int> UsedNumbers { get; set; }
        public List<(int row, int col, int? number)> MoveHistory { get; set; } // Tracks the history of moves made during the game, including row, column, and optional number for Numerical Tic-Tac-Toe
        public int CurrentMoveIndex { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public bool IsPlayerVsPlayer { get; set; }

        // Parameterless constructor for serialization
        public GameState()
        {

        }

        public GameState(string name, Player currentPlayer, Player player1, Player player2, List<int> usedNumbers, List<(int row, int col, int? number)> moveHistory, int currentMoveIndex, int height, int width, bool isPlayerVsPlayer) // Constructor to initialize a new game state
        {
            Name = name;
            CurrentPlayer = currentPlayer;
            Player1 = player1;
            Player2 = player2;
            UsedNumbers = usedNumbers;
            MoveHistory = moveHistory;
            CurrentMoveIndex = currentMoveIndex;
            Height = height;
            Width = width;
            IsPlayerVsPlayer = isPlayerVsPlayer;
        }

        public void ReconstructBoard() // Method to reconstruct the board from the move history after loading the game state
        {
            Board = new Board(Height, Width);
            char piece;
            foreach (var move in MoveHistory)
            {
                piece = Convert.ToChar(move.number + '0');
                Board.PlacePiece(piece, move.row, move.col);
            }
            ;
        }

    }

    public static class GameStateManager //Class for saving and loading game  
    {
        public static void SaveGame(GameState gameState, string filePath) // Method to save the game state to a specified file path
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameState)); // Create a serializer for the GameState type
            using (StreamWriter writer = new StreamWriter(filePath)) // Use StreamWriter to write the serialized object to a file
            { 
                serializer.Serialize(writer, gameState); // Serialize the game state and write it to the file
            }
        }

        public static GameState LoadGame() // Method to load a game state from a saved file
        {
            string saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game", "SaveFiles");
            if (!Directory.Exists(saveDirectory))
            {
                Console.WriteLine("Save directory not found. Returning to menu...");
                Console.ReadKey();
                return null;
            }

            var saveFiles = Directory.GetFiles(saveDirectory, "*.xml"); // Get all XML save files from the directory
            if (saveFiles.Length == 0)
            {
                Console.WriteLine("No save files found. Press any key to continue. ");
                Console.ReadKey();
                return null;
            }

            Console.WriteLine("Available save files:");  // Display available save files to the user
            for (int i = 0; i < saveFiles.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(saveFiles[i])}"); //Display save file names without their extensions
            }

            int fileIndex;
            while (true)
            {
                Console.Write("Enter the number of the save file to load: ");
                if (int.TryParse(Console.ReadLine(), out fileIndex) && fileIndex > 0 && fileIndex <= saveFiles.Length) //Validates user input to make sure it corresponds to a correct file 
                {
                    break;
                }
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }

            string filePath = saveFiles[fileIndex - 1]; // Get the selected save file path based on the user's input

            XmlSerializer serializer = new XmlSerializer(typeof(GameState)); // Re-create a serializer for the GameState type
            using (StreamReader reader = new StreamReader(filePath)) // Use StreamReader to read the serialized object from the file
            {
                var gameState = (GameState?)serializer.Deserialize(reader); // Deserialize the game state from the file
                if (gameState == null)
                {
                    throw new InvalidOperationException("Failed to deserialize game state."); 
                }
                gameState.ReconstructBoard(); // Reconstruct the board from the move history

                if (gameState.Player2 is AIPlayer aiPlayer2)  //Checks if player 2 is AIPlayer 
                {
                    aiPlayer2.InitializeLoadAIPlayer(); //Re-initialize the AI player if it exists
                }

                return gameState; // Return the loaded game state
            }
        }
    }
}