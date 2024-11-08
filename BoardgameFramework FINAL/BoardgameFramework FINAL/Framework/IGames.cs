using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardgameFramework
{
    public interface IGame
    {
        void PlayGame(bool isPlayerVsPlayer, GameState? loadedGameState = null); // Method to play the game - isPlayerVsPlayer indicates the game mode, loadedGameState: parameter for loading a saved game state, allowing for the option to load a previous game state. 
        string Name { get; }  // Property to get the name of the game
    }
}
