using System;
using System.IO;

namespace cslox
{
    public static class Lox
    {
        private static bool hadError;

        public static void RunFile(string path)
        {
            var source = File.ReadAllText(path);
            Run(source);

            if (hadError) Environment.Exit(65);
        }

        public static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                Run(Console.ReadLine());
                hadError = false;
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
                Report(token.Line, " at end", message);
            else
                Report(token.Line, $" at '{token.Lexeme}'", message);
        }

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var expr = parser.Parse();

            if (hadError) return;

            Console.WriteLine(new AstPrinter().Print(expr));
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine("[line {0}] Error{1}: {2}", line, where, message);
            hadError = true;
        }
    }
}