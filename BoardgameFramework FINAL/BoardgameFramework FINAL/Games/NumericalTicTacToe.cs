using BoardgameFramework;

namespace BoardgameFramework.Game
{
    internal class NumericalTicTacToe : IGame
    {
        public string Name => "Numerical Tic Tac Toe";

        private Board board;
        private Player playerOdd;
        private Player playerEven;
        private Player currentPlayer;
        private List<int> usedNumbers; 

        private List<(int row, int col, int? number)> moveHistory = new List<(int row, int col, int? number)>(); // List to track move history and the current move index
        private int currentMoveIndex = -1; //Index to track the current move in the history
        private bool isPlayerVsPlayer; // Boolean to track if the game is Player vs Player

        public NumericalTicTacToe() // Constructor to initialize the game 
        {
            board = new Board(3, 3);
            playerOdd = new HumanPlayer('O'); // 'O' for Odd
            playerEven = new HumanPlayer('E'); // 'E' for Even
            currentPlayer = playerOdd;
            usedNumbers = new List<int>();
        }

        public void PlayGame(bool isPlayerVsPlayer, GameState? loadedGameState = null)  // Method to start or resume a game
        {
            if (loadedGameState != null)   // If loading a game from saved file, load the game state
            {
                board = loadedGameState.Board;
                playerOdd = loadedGameState.Player1;
                playerEven = loadedGameState.Player2;
                if (loadedGameState.CurrentPlayer.Symbol == (79)) //79 = 'O' (represents Odd). The code is implemented this way because the player’s symbol is stored as its ASCII value (79 for 'O') in the XML file. 
                {
                    currentPlayer = playerOdd;
                }
                else 
                { 
                    currentPlayer = playerEven;
                }
                usedNumbers = loadedGameState.UsedNumbers;
                moveHistory = loadedGameState.MoveHistory;
                currentMoveIndex = loadedGameState.CurrentMoveIndex;
                isPlayerVsPlayer = loadedGameState.IsPlayerVsPlayer;
            }
            else // Initialize a new game
            {
                playerOdd = new HumanPlayer('O');
                playerEven = isPlayerVsPlayer ? (Player)new HumanPlayer('E') : new AIPlayer('E');
                currentPlayer = playerOdd;
                board = new Board(3, 3); // Reset the board for a new game
                usedNumbers.Clear(); // Reset used numbers
                this.isPlayerVsPlayer = isPlayerVsPlayer;
            }

            bool playAgain;
            do
            {
                if (loadedGameState == null)
                {
                    Console.Clear();
                    Console.WriteLine("Welcome to Numerical Tic Tac Toe!");
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine("First to get numbers summing to 15 in a row wins!");
                    Console.WriteLine("Player 'O' can only play odd numbers from 1-9");
                    Console.WriteLine("Player 'E' can only play even numbers from 1-9");
                    Console.WriteLine("Press any key to start...");
                    Console.ReadKey();
                }

                bool gameWon = false;
                var gameRules = Rules.GetNumericalTicTacToeRules(); //Display rule of NumericalTicTacToe

                while (!gameWon && usedNumbers.Count < 9) // Continue until a player wins or all numbers are used
                {
                    Console.Clear();
                    board.DisplayBoard();
                    Console.WriteLine($"Player {currentPlayer.Symbol}, enter your move:");

                    var (row, col) = currentPlayer.GetMove(board, gameRules);

                    if (row == -3 && col == -3)  // Handle special commands like save, undo, or redo - used in HumanPlayer class
                    {
                        SaveGame();
                        continue;
                    }
                    else if (row == -1 && col == -1)
                    {
                        UndoMove();
                        continue;
                    }
                    else if (row == -2 && col == -2)
                    {
                        RedoMove();
                        continue;
                    }

                    int number = currentPlayer is AIPlayer aiPlayer ? aiPlayer.GetValidNumber() : GetValidNumber(); // Get a valid number for the move

                    try
                    {
                        PlacePiece(row, col, number);
                        if (currentMoveIndex < moveHistory.Count - 1) //if not at the end of the moveHistory list
                        {
                            moveHistory.RemoveRange(currentMoveIndex + 1, moveHistory.Count - currentMoveIndex - 1); // Remove future moves if undo was used, this is because a new branch of moves can potentially be created which invalides moves that were previously made
                        }
                        moveHistory.Add((row, col, number)); // Add the move to the history
                        currentMoveIndex++; //Increment to track move index
                        gameWon = CheckWin(); // Check if the move resulted in a win
                        if (!gameWon)
                        {
                            SwitchPlayer();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press any key to try again...");
                        Console.ReadKey();
                    }
                }

                Console.Clear();
                board.DisplayBoard();
                if (gameWon)
                {
                    Console.WriteLine($"Player {currentPlayer.Symbol} wins!");
                }
                else
                {
                    Console.WriteLine("It's a draw!");
                }

                playAgain = AskToPlayAgain();
            } while (playAgain);
        }

        private void ResetGame()  // Method to reset the game state for a new game
        {
            board = new Board(3, 3);
            playerOdd = new HumanPlayer('O'); // 'O' for Odd
            playerEven = isPlayerVsPlayer ? (Player)new HumanPlayer('E') : new AIPlayer('E'); // 'E' for Even
            currentPlayer = playerOdd;
            usedNumbers.Clear();
            moveHistory.Clear();
            currentMoveIndex = -1;
        }

        private void SaveGame() // Method to save the current game state of NumericalTicTacToe
        {
            string saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game", "SaveFiles");
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            Console.Write("Enter the file name to save the game: ");
            string? fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName)) // Validate the file name
            {
                Console.WriteLine("Invalid file name. Game not saved.");
                return;
            }
            string filePath = Path.Combine(saveDirectory, fileName + ".xml"); // Create the full file path

            var gameState = new GameState(Name, currentPlayer, playerOdd, playerEven, usedNumbers, moveHistory, currentMoveIndex, board.height, board.width, isPlayerVsPlayer); //create a new GameState object
            GameStateManager.SaveGame(gameState, filePath); //saves game
            Console.WriteLine("Game saved successfully. Press any key to continue...");
            Console.ReadKey();
        }

        private int GetValidNumber()  // Method to get a valid number from the player
        {
            int number;
            while (true)
            {
                Console.Write("Enter a number: ");
                if (int.TryParse(Console.ReadLine(), out number) && IsValidNumber(number))
                {
                    usedNumbers.Add(number); //inputted number added to usedNumbers list to avoid duplication 
                    return number;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
        }

        private bool IsValidNumber(int number)   // Method to validate the number input by the player
        {
            if (currentPlayer == playerOdd && number % 2 == 1 && number >= 1 && number <= 9 && !usedNumbers.Contains(number)) // Check if the number is odd
            {
                return true;
            }
            if (currentPlayer == playerEven && number % 2 == 0 && number >= 2 && number <= 8 && !usedNumbers.Contains(number)) // Check if the number is even
            {
                return true;
            }
            return false;
        }

        private void PlacePiece(int row, int col, int number) // Method to place a piece on the board
        {
            board.PlacePiece(number.ToString()[0], row, col);
        }

        private void SwitchPlayer()  // Method to switch the current player
        {
            currentPlayer = currentPlayer == playerOdd ? playerEven : playerOdd;
        }

        private bool CheckWin() // Method to check if the current player has won the game
        {
            // Check rows, columns, and diagonals for a sum of 15
            for (int i = 0; i < 3; i++)
            {
                if (SumRow(i) == 15 || SumColumn(i) == 15) // Return true if a row or column sums to 15
                    return true;
            }

            if (SumDiagonal1() == 15 || SumDiagonal2() == 15) //Return true if diagonal sums to 15
                return true;

            return false;
        }

        private int SumRow(int row) 
        {
            return Sum(board[row, 0]) + Sum(board[row, 1]) + Sum(board[row, 2]);
        }

        private int SumColumn(int col) 
        {
            return Sum(board[0, col]) + Sum(board[1, col]) + Sum(board[2, col]);
        }

        private int SumDiagonal1()
        {
            return Sum(board[0, 0]) + Sum(board[1, 1]) + Sum(board[2, 2]);
        }

        private int SumDiagonal2()
        {
            return Sum(board[0, 2]) + Sum(board[1, 1]) + Sum(board[2, 0]);
        }

        private int Sum(char c) // Method to convert a board cell to its integer value
        { 
            return c == '.' ? 0 : int.Parse(c.ToString()); // Convert the character to an integer, or return 0 for empty cells
        }

        private bool AskToPlayAgain()  // Method to ask the user if they want to play again
        {
            Console.WriteLine("Do you want to play again? (y/n)");
            while (true)
            {
                var input = Console.ReadLine()?.ToLower() ?? string.Empty;
                if (input == "y")
                {
                    ResetGame();
                    return true;
                }
                else if (input == "n")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'y' to play again or 'n' to exit.");
                }
            }
        }

        private void UndoMove() // Method to undo the last move
        {
            if (currentMoveIndex >= 0)
            {
                var lastMove = moveHistory[currentMoveIndex];
                board.PlacePiece('.', lastMove.row, lastMove.col); // Remove the piece
                currentMoveIndex--; //decremented to track currentMove number
                SwitchPlayer();
            }
            else
            {
                Console.WriteLine("No moves to undo.");
            }
        }

        private void RedoMove() // Method to redo the last undone move
        {
            if (currentMoveIndex < moveHistory.Count - 1)
            {
                currentMoveIndex++; //incremented to track currentMove number
                var moveToRedo = moveHistory[currentMoveIndex];
                char piece = Convert.ToChar(moveToRedo.number + '0');
                board.PlacePiece(piece, moveToRedo.row, moveToRedo.col); // Place the piece back
                SwitchPlayer();
            }
            else
            {
                Console.WriteLine("No moves to redo.");
            }
        }

        public static class Rules  // Static class to define the game rules
        {
            public static GameRules GetNumericalTicTacToeRules()
            {
                return new GameRules
                {
                    Title = "Numerical Tic Tac Toe Rules:",
                    Rules = new List<string>
                    {
                        "1. The game is played on a 3x3 grid.",
                        "2. Players take turns to place a number from 1 to 9 on the grid.",
                        "3. Each number can only be used once.",
                        "4. The first player to get three numbers in a row, column, or diagonal that add up to 15 wins.",
                        "5. Enter 'u' to undo the last move.",
                        "6. Enter 'r' to redo the last undone move.",
                        "7. Enter 'h' to display these rules.",
                        "8. Enter 's' to save the game."
                    }
                };
            }
        }
    }
}