using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Net_pracrice;

namespace _.Net_pracrice
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowHeight = 55;
            Console.WindowWidth = 135;
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.BufferWidth;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;            
            Console.Clear();
            Console.Title = "WCMD manager  -  © Podkolzin Andrey 2012";
           
            new Commander();
        }
    }
}
