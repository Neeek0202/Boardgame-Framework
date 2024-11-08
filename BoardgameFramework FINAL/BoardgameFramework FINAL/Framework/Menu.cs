using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace BoardgameFramework
{
    public static class Menu
    {
        public static void Start() // Entry point for the menu system, providing options to start a new game, load a game, or exit
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Board Game Console App!");
                Console.WriteLine("Please choose from the following:");
                Console.WriteLine("1. Start New Game");
                Console.WriteLine("2. Load Game");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowGameOptions(); // Show available games to start
                        break;
                    case "2":
                        var gameState = GameStateManager.LoadGame(); // Load a saved game state
                        if (gameState != null)
                        {
                            LoadGame(gameState); // Load the game with the selected state
                        }
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void ShowGameOptions() // Method to display available games flexibly and allow the user to select one to play
        {
            var games = Assembly.GetExecutingAssembly() // Use reflection to find all types implementing the IGame interface, instantiate them, and store them in a list
                                .GetTypes()
                                .Where(t => typeof(IGame).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                .Select(t => (IGame)Activator.CreateInstance(t))
                                .ToList();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select a game from the following options:");
                for (int i = 0; i < games.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {games[i].Name}"); // List available games
                }
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                if (int.TryParse(choice, out int gameIndex) && gameIndex > 0 && gameIndex <= games.Count) // Validate the user's choice and start the selected game
                {
                    var selectedGame = games[gameIndex - 1];
                    Console.Clear();
                    Console.WriteLine("Select game mode:");
                    Console.WriteLine("1. Player vs Player");
                    Console.WriteLine("2. Player vs Computer");
                    Console.Write("Enter your choice: ");
                    string modeChoice = Console.ReadLine();

                    if (int.TryParse(modeChoice, out int mode) && (mode == 1 || mode == 2)) // Validate the game mode choice and start the game with the appropriate mode i.e. pvp or pvc 
                    {
                        selectedGame.PlayGame(mode == 1); //Calls PlayGame() method for PVP mode. 
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ReadKey();
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.ReadKey();
                }
            }
        }

        private static void LoadGame(GameState gameState) // Method to load a game using a saved GameState object. This method allows for dynamic loading of future games that may be added to the framework
        {
            var games = Assembly.GetExecutingAssembly() // Use reflection to find all types implementing the IGame interface, instantiate them, and store them in a list
                             .GetTypes()
                             .Where(t => typeof(IGame).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                             .Select(t => (IGame)Activator.CreateInstance(t))
                             .ToList();
            var savedGameName = gameState.Name; // Find the correct game that matches the saved game's name and load it
            foreach (var game in games)
            {
                if (game.Name == savedGameName) 
                {
                    var loadGame = (IGame)Activator.CreateInstance(game.GetType());
                    loadGame.PlayGame(gameState.IsPlayerVsPlayer, gameState); // Start the game with the loaded state
                }
            }
        }
    }
}

