using System;
using System.IO;

namespace cslox
{
    class Program
    {
        private static bool hadError;

        static void Main(string[] args)
        {
            if (args.Length > 1)
                Console.WriteLine("Usage: cslox [script]");
            else if (args.Length == 1)
                RunFile(args[0]);
            else
                RunPrompt();
        }

        private static void RunFile(string path)
        {
            var source = File.ReadAllText(path);
            Run(source);

            if (hadError) Environment.Exit(65);
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                Run(Console.ReadLine());
                hadError = false;
            }
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine("[line {0}] Error{1}: {1}", line, where, message);
            hadError = true;
        }
    }
}
