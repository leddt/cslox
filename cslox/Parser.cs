using System.Collections.Generic;
using System.Linq;

using static cslox.TokenType;
using static cslox.Expr;

namespace cslox
{
    /*
       expression     → equality ;
       equality       → comparison ( ( "!=" | "==" ) comparison )* ;
       comparison     → addition ( ( ">" | ">=" | "<" | "<=" ) addition )* ;
       addition       → multiplication ( ( "-" | "+" ) multiplication )* ;
       multiplication → unary ( ( "/" | "*" ) unary )* ;
       unary          → ( "!" | "-" ) unary
                      | primary ;
       primary        → NUMBER | STRING | "false" | "true" | "nil"
                      | "(" expression ")" ;
     */

    public class Parser
    {
        private readonly List<Token> tokens;
        private int current;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParserErrorException)
            {
                return null;
            }
        }

        private Expr Expression()
        {
            return Equality();
        }

        private Expr Equality()
        {
            var expr = Comparison();

            while (Match(BangEqual, EqualEqual))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            var expr = Addition();

            while (Match(Greater, GreaterEqual, Less, LessEqual))
            {
                var op = Previous();
                var right = Addition();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Addition()
        {
            var expr = Multiplication();

            while (Match(Minus, Plus))
            {
                var op = Previous();
                var right = Multiplication();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Multiplication()
        {
            var expr = Unary();

            while (Match(Slash, Star))
            {
                var op = Previous();
                var right = Unary();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }


        private Expr Unary()
        {
            if (Match(Bang, Minus))
            {
                var op = Previous();
                var right = Unary();
                return new Unary(op, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(False)) return new Literal(false);
            if (Match(True)) return new Literal(true);
            if (Match(Nil)) return new Literal(null);

            if (Match(Number, String)) return new Literal(Previous().Literal);

            if (Match(LeftParen))
            {
                var expr = Expression();
                Consume(RightParen, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }


        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private ParserErrorException Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParserErrorException();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == Semicolon) return;

                switch (Peek().Type)
                {
                    case Class:
                    case Fun:
                    case Var:
                    case For:
                    case If:
                    case While:
                    case Print:
                    case Return:
                        return;
                }

                Advance();
            }
        }



        private bool Match(params TokenType[] types)
        {
            if (!types.Any(Check)) return false;

            Advance();
            return true;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private bool IsAtEnd() => Peek().Type == EOF;

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private Token Peek() => tokens[current];
        private Token Previous() => tokens[current - 1];


        public class ParserErrorException : System.Exception {}
    }
}