using System;
using System.CodeDom.Compiler;

namespace StS
{
    /// <summary>
    /// player > gamecontext > fight
    /// </summary>
    public class GameContext
    {
        public Player Player { get; set; }

        public GameContext()
        {
            
        }
        public void GameOver()
        {
            Console.WriteLine("Game over.");
        }
    }
}