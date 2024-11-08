using System;
using System.Collections.Generic;

namespace BoardgameFramework
{
    public class GameRules //creating a Help class allows for flexibility for adapting to different game rules 
    {
        public string Title { get; set; } 
        public List<string> Rules { get; set; } = new List<string>(); //holds the rule of game 

        public void DisplayRules()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(Title);
            foreach (var rule in Rules)
            {
                Console.WriteLine(rule);
            }
            Console.WriteLine("Press any key to return to the game...");
            Console.ReadKey();
        }
    }
}