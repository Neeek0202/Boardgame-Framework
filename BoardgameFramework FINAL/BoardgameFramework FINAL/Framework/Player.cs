using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BoardgameFramework
{
    [Serializable]  // Marks the class as serializable and includes HumanPlayer and AIPlayer in the XML serialization process
    [XmlInclude(typeof(HumanPlayer))]
    [XmlInclude(typeof(AIPlayer))]
    public abstract class Player
    {
        public char Symbol { get; set; }

        protected Player() { }

        protected Player(char symbol)
        {
            Symbol = symbol;
        }

        public abstract (int row, int col) GetMove(Board board, GameRules gameRules);  // Abstract method to be implemented by subclasses HumanPlayer and AIPlayer to get a move on the board. Returns a tuple repesenting the move based on the board's state and game rules 
    }
}
