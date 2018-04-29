using System.Collections.Generic;
using System.Linq;

using static cslox.TokenType;
using static cslox.Expr;

namespace cslox
{
    /*
        program        → declaration* EOF ;
                      
        declaration    → funDecl
                       | varDecl
                       | statement ;
                      
        funDecl        → "fun" IDENTIFIER "(" parameters? ")" block ;
        parameters     → IDENTIFIER ( "," IDENTIFIER )* ;
        varDecl        → "var" IDENTIFIER ( "=" expression )? ";" ;
        statement      → exprStmt
                       | forStmt
                       | ifStmt
                       | printStmt
                       | returnStmt
                       | whileStmt
                       | block;
                      
        exprStmt       → expression ";" ;
        forStmt        → "for" "(" ( varDecl | exprStmt | ";" )
                                   expression? ";"
                                   expression? ")" statement ;
        ifStmt         → "if" "(" expression ")" statement ( "else" statement )? ;
        printStmt      → "print" expression ";" ;
        returnStmt     → "return" expression? ";" ;
        whileStmt      → "while" "(" expression ")" statement ;
        block          → "{" declaration* "}" ;
        
        expression     → assignment ;
        assignment     → identifier "=" assignment
                       | logic_or ;
        logic_or       → logic_and ( "or" logic_and )* ;
        logic_and      → equality ( "and" equality ) ;
        equality       → comparison ( ( "!=" | "==" ) comparison )* ;
        comparison     → addition ( ( ">" | ">=" | "<" | "<=" ) addition )* ;
        addition       → multiplication ( ( "-" | "+" ) multiplication )* ;
        multiplication → unary ( ( "/" | "*" ) unary )* ;
        unary          → ( "!" | "-" ) unary
                       | call ;
        call           → primary ( "(" arguments? ")" )* ;
        arguments      → expression ( "," expression )* ;
        primary        → "true" | "false" | "nil" | "this"
                       | NUMBER | STRING
                       | "(" expression ")"
                       | IDENTIFIER ;
     */

    public class Parser
    {
        private readonly Token[] tokens;
        private int current;

        public Parser(Token[] tokens)
        {
            this.tokens = tokens;
        }

        public Stmt[] Parse()
        {
            var statements = new List<Stmt>();

            while (!IsAtEnd())
                statements.Add(Declaration());

            return statements.ToArray();
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(Fun)) return FunctionDeclaration("function");
                if (Match(Var)) return VarDeclaration();
                return Statement();
            }
            catch (ParserErrorException)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt FunctionDeclaration(string kind)
        {
            var name = Consume(Identifier, $"Expect {kind} name.");

            Consume(LeftParen, $"Expect '(' after {kind} name.");

            var parameters = new List<Token>();
            if (!Check(RightParen))
            {
                do
                {
                    if (parameters.Count > 8) Error(Peek(), "Cannot have more than 8 parameters.");
                    parameters.Add(Consume(Identifier, "Expect parameter name."));
                } while (Match(Comma));
            }

            Consume(RightParen, $"Expect ')' after parameters.");

            Consume(LeftBrace, $"Expect '{{' before {kind} body.");
            var body = BlockStatement();

            return new Stmt.Function(name, parameters.ToArray(), body.Statements);
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(Identifier, "Expect variable name.");

            var initializer = Match(Equal) 
                ? Expression() 
                : null;

            Consume(Semicolon, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(For)) return ForStatement();
            if (Match(If)) return IfStatement();
            if (Match(Print)) return PrintStatement();
            if (Match(Return)) return ReturnStatement();
            if (Match(While)) return WhileStatement();
            if (Match(LeftBrace)) return BlockStatement();
            return ExpressionStatement();
        }

        private Stmt ForStatement()
        {
            Consume(LeftParen, "Expect '(' after 'for'.");

            Stmt initializer;
            if (Match(Semicolon))
                initializer = null;
            else if (Match(Var))
                initializer = VarDeclaration();
            else
                initializer = ExpressionStatement();

            var condition = Check(Semicolon) ? null : Expression();
            Consume(Semicolon, "Expect ';' after loop condition.");

            var increment = Check(RightParen) ? null : Expression();
            Consume(RightParen, "Expect ')' after for clauses.");

            var body = Statement();

            if (increment != null)
            {
                body = new Stmt.Block(new [] {
                    body,
                    new Stmt.Expression(increment)
                });
            }

            if (condition == null) condition = new Literal(true);
            body = new Stmt.While(condition, body);

            if (initializer != null)
            {
                body = new Stmt.Block(new [] {
                    initializer,
                    body
                });
            }

            return body;
        }

        private Stmt IfStatement()
        {
            Consume(LeftParen, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(RightParen, "Expect ')' after if condition.");

            var thenBranch = Statement();
            var elseBranch = Match(Else)
                ? Statement()
                : null;

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt PrintStatement()
        {
            var value = Expression();
            Consume(Semicolon, "Expect ';' after value.");

            return new Stmt.Print(value);
        }

        private Stmt ReturnStatement()
        {
            var keyword = Previous();
            var value = Check(Semicolon)
                ? null
                : Expression();

            Consume(Semicolon, "Expect ';' after return value.");

            return new Stmt.Return(keyword, value);
        }

        private Stmt WhileStatement()
        {
            Consume(LeftParen, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(RightParen, "Expect ')' after while condition.");

            var body = Statement();

            return new Stmt.While(condition, body);
        }

        private Stmt.Block BlockStatement()
        {
            var statements = new List<Stmt>();

            while (!Check(RightBrace) && !IsAtEnd())
                statements.Add(Declaration());

            Consume(RightBrace, "Expect '}' after block.");

            return new Stmt.Block(statements.ToArray());
        }

        private Stmt ExpressionStatement()
        {
            var expr = Expression();
            Consume(Semicolon, "Expect ';' after expression.");

            return new Stmt.Expression(expr);
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            var expr = Or();

            if (Match(Equal))
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is Variable varExpr)
                {
                    var name = varExpr.Name;
                    return new Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            var expr = And();

            while (Match(TokenType.Or))
            {
                var op = Previous();
                var right = And();
                expr = new Logical(expr, op, right);
            }

            return expr;
        }

        private Expr And()
        {
            var expr = Equality();

            while (Match(TokenType.And))
            {
                var op = Previous();
                var right = Equality();
                expr = new Logical(expr, op, right);
            }

            return expr;
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

            return Call();
        }

        private Expr Call()
        {
            var expr = Primary();

            while (true)
            {
                if (Match(LeftParen))
                    expr = FinishCall(expr);
                else
                    break;
            }

            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            var arguments = new List<Expr>();
            if (!Check(RightParen))
            {
                do
                {
                    if (arguments.Count > 8) Error(Peek(), "Cannot have more than 8 arguments.");

                    arguments.Add(Expression());
                } while (Match(Comma));
            }

            var paren = Consume(RightParen, "Expect ')' after arguments.");

            return new Call(callee, paren, arguments.ToArray());
        }

        private Expr Primary()
        {
            if (Match(False)) return new Literal(false);
            if (Match(True)) return new Literal(true);
            if (Match(Nil)) return new Literal(null);

            if (Match(Number, String)) return new Literal(Previous().Literal);

            if (Match(Identifier)) return new Variable(Previous());

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