using System;
using System.IO;

namespace cslox
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length > 1)
                Console.WriteLine("Usage: cslox [script]");
            else if (args.Length == 1)
                Lox.RunFile(args[0]);
            else
                Lox.RunPrompt();
        }
    }
}
