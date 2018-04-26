using System.Collections.Generic;
using System.Globalization;

using static cslox.TokenType;

namespace cslox
{
    public class Scanner
    {
        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType> {
            {"and", And},
            {"class", Class},
            {"else", Else},
            {"false", False},
            {"for", For},
            {"fun", Fun},
            {"if", If},
            {"nil", Nil},
            {"or", Or},
            {"print", Print},
            {"return", Return},
            {"super", Super},
            {"this", This},
            {"true", True},
            {"var", Var},
            {"while", While}
        };

        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();

        private int start;
        private int current;
        private int line = 1;

        public Scanner(string source)
        {
            this.source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }

        private void ScanToken()
        {
            var c = Advance();

            switch (c)
            {
                case '(': AddToken(LeftParen); break;
                case ')': AddToken(RightParen); break;
                case '{': AddToken(LeftBrace); break;
                case '}': AddToken(RightBrace); break;
                case ',': AddToken(Comma); break;
                case '.': AddToken(Dot); break;
                case '-': AddToken(Minus); break;
                case '+': AddToken(Plus); break;
                case ';': AddToken(Semicolon); break;
                case '*': AddToken(Star); break;

                case '!': AddToken(Match('=') ? BangEqual : Bang); break;
                case '=': AddToken(Match('=') ? EqualEqual : Equal); break;
                case '<': AddToken(Match('=') ? LessEqual : Less); break;
                case '>': AddToken(Match('=') ? GreaterEqual : Greater); break;

                case '/':
                    if (Match('/'))
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    else
                        AddToken(Slash);

                    break;

                case '"': ScanString(); break;

                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;

                default:
                    if (IsDigit(c))
                        ScanNumber();
                    else if (IsAlpha(c))
                        ScanIdentifier();
                    else
                        Lox.Error(line, $"Unexpected character: {c}");

                    break;
            }
        }

        private void ScanString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            Advance();

            var value = Substring(start + 1, current - 1);
            AddToken(String, value);
        }

        private void ScanNumber()
        {
            while (IsDigit(Peek())) Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();
                while (IsDigit(Peek())) Advance();
            }

            AddToken(Number, double.Parse(Substring(start, current), CultureInfo.InvariantCulture));
        }

        private void ScanIdentifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            var text = Substring(start, current);

            if (Keywords.TryGetValue(text, out var tokenType))
                AddToken(tokenType);
            else
                AddToken(Identifier);
        }


        private char Advance()
        {
            current++;
            return source[current - 1];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        private char Peek(int offset = 0)
        {
            return IsAtEnd(offset) ? '\0' : source[current + offset];
        }

        private char PeekNext() => Peek(offset: 1);

        private void AddToken(TokenType type, object literal = null)
        {
            var text = Substring(start, current);
            tokens.Add(new Token(type, text, literal, line));
        }


        private string Substring(int startIndex, int endIndex) => source.Substring(startIndex, endIndex - startIndex);

        private bool IsAtEnd(int offset = 0) => current + offset >= source.Length;
        private bool IsDigit(char c) => c >= '0' && c <= '9';
        private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') ||
                                        (c >= 'A' && c <= 'Z') ||
                                        (c == '_');
        private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);
    }
}