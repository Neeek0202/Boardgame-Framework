using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardgameFramework
{
    [Serializable] //Marks the class Piece as serializable 
    public class Piece
    {
        public char Symbol { get; private set; }  // The symbol represents the player's piece on the board (e.g., 'X', 'O', or '1'-'9').

        public Piece(char symbol)
        {
            Symbol = symbol;
        }
    }
}