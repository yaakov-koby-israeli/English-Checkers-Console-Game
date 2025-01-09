using System;
using Ex02.ConsoleUtils;

namespace Ex02
{
    internal class Program
    {
        public static void Main()
        {
            UserInterface game = new UserInterface();
            game.StartGame();

            Console.WriteLine("Game Over !!! ");

        }
    }
}
