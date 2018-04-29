using System;
using System.IO;

namespace cslox
{
    public static class Lox
    {
        private static bool hadError;
        private static bool hadRuntimeError;

        private static readonly Interpreter interpreter = new Interpreter();

        public static void RunFile(string path)
        {
            var source = File.ReadAllText(path);
            Run(source);

            if (hadError) System.Environment.Exit(65);
            if (hadRuntimeError) System.Environment.Exit(70);
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

        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var statements = parser.Parse();

            if (hadError) return;

            var resolver = new Resolver(interpreter);
            resolver.Resolve(statements);
            
            if (hadError) return;

            interpreter.Interpret(statements);
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

        public static void RuntimeError(RuntimeErrorException error)
        {
            Console.WriteLine(error.Message);
            Console.WriteLine($"[line {error.Token.Line}]");

            hadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine("[line {0}] Error{1}: {2}", line, where, message);
            hadError = true;
        }
    }
}