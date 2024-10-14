using BattleshipAppLibrary;
using BattleshipAppLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleUI.InitGame();
            ConsoleUI.RunGame();
            ConsoleUI.ExitGame();

        }
    }
}
