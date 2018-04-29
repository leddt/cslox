using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using static cslox.Expr;
using static cslox.TokenType;

namespace cslox
{
    public class Interpreter : IExprVisitor<object>, Stmt.IStmtVisitor<Void>
    {
        public readonly Environment Globals = new Environment();
        private Environment environment;

        public Interpreter()
        {
            environment = Globals;

            Globals.Define("clock", new NativeFunction(0, (i, args) => DateTime.Now.Ticks / TimeSpan.TicksPerSecond));
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var stmt in statements)
                    Execute(stmt);
            }
            catch (RuntimeErrorException error)
            {
                Lox.RuntimeError(error);
            }
        }


        public Void Visit(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(environment));

            return Void.Instance;
        }

        public Void Visit(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);

            return Void.Instance;
        }

        public Void Visit(Stmt.Function stmt)
        {
            var function = new LoxFunction(stmt, environment);
            environment.Define(stmt.Name.Lexeme, function);

            return Void.Instance;
        }

        public Void Visit(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
                Execute(stmt.ThenBranch);
            else if (stmt.ElseBranch != null)
                Execute(stmt.ElseBranch);

            return Void.Instance;
        }

        public Void Visit(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.Expr);
            Console.WriteLine(Stringify(value));

            return Void.Instance;
        }

        public Void Visit(Stmt.Return stmt)
        {
            var value = stmt.Value != null ? Evaluate(stmt.Value) : null;

            throw new ReturnException(value);
        }

        public Void Visit(Stmt.Var stmt)
        {
            var value = stmt.Initializer != null
                ? Evaluate(stmt.Initializer)
                : null;

            environment.Define(stmt.Name, value);

            return Void.Instance;
        }

        public Void Visit(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
                Execute(stmt.Body);

            return Void.Instance;
        }


        public object Visit(Assign expr)
        {
            var value = Evaluate(expr.Value);

            environment.Assign(expr.Name, value);
            return value;
        }

        public object Visit(Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            double ld, rd;

            switch (expr.Op.Type)
            {
                case Minus:
                    (ld, rd) = CheckNumberOperands(expr.Op, left, right);
                    return ld - rd;
                case Slash:
                    (ld, rd) = CheckNumberOperands(expr.Op, left, right);
                    if (rd == 0) throw new RuntimeErrorException(expr.Op, "Attempt to divide by zero.");

                    return ld / rd;
                case Star:
                    (ld, rd) = CheckNumberOperands(expr.Op, left, right);
                    return ld * rd;

                case Plus:
                    if (TryCheckOperands<double>(left, right, out var plusNumbers))
                        return plusNumbers.left + plusNumbers.right;

                    if (TryCheckOperands<string>(left, right, out var plusStrings))
                        return string.Concat(plusStrings.left, plusStrings.right);

                    throw new RuntimeErrorException(expr.Op, "Operands must be two numbers or two strings.");

                case Greater:
                    (ld, rd) = CheckNumberOperands(expr.Op, left, right);
                    return ld > rd;
                case GreaterEqual:
                    (ld, rd) = CheckNumberOperands(expr.Op, left, right);
                    return ld >= rd;
                case Less:
                    (ld, rd) = CheckNumberOperands(expr.Op, left, right);
                    return ld < rd;
                case LessEqual:
                    (ld, rd) = CheckNumberOperands(expr.Op, left, right);
                    return ld <= rd;

                case EqualEqual: return IsEqual(left, right);
                case BangEqual: return !IsEqual(left, right);
            }

            return null;
        }

        public object Visit(Call expr)
        {
            var callee = Evaluate(expr.Callee);
            var arguments = expr.Arguments.Select(Evaluate).ToList();

            if (!(callee is LoxCallable function))
                throw new RuntimeErrorException(expr.Paren, "Can only call functions and classes.");

            if (arguments.Count != function.Arity)
                throw new RuntimeErrorException(expr.Paren, $"Expected {function.Arity} arguments but got {arguments.Count}.");

            return function.Call(this, arguments);
        }

        public object Visit(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object Visit(Literal expr)
        {
            return expr.Value;
        }

        public object Visit(Logical expr)
        {
            var left = Evaluate(expr.Left);

            if (expr.Op.Type == Or)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
        }

        public object Visit(Unary expr)
        {
            var right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case Minus:
                    var number = CheckNumberOperand(expr.Op, right);
                    return -number;
                case Bang:
                    return !IsTruthy(right);
            }

            return null;
        }

        public object Visit(Variable expr)
        {
            return environment.Get(expr.Name);
        }

        private void Execute(Stmt stmt) => stmt.Accept(this);

        public void ExecuteBlock(List<Stmt> statements, Environment env)
        {
            var previousEnv = environment;

            try
            {
                environment = env;
                foreach (var stmt in statements)
                    Execute(stmt);
            }
            finally
            {
                environment = previousEnv;
            }
        }

        private object Evaluate(Expr expr) => expr.Accept(this);

        private bool IsTruthy(object value)
        {
            if (value == null) return false;
            if (value is bool b) return b;

            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        private string Stringify(object value)
        {
            if (value == null) return "nil";
            if (value is bool b) return b ? "true" : "false";
            return value.ToString();
        }

        private double CheckNumberOperand(Token @operator, object operand)
        {
            if (operand is double d) return d;
            throw new RuntimeErrorException(@operator, "Operand must be a number.");
        }

        private (double, double) CheckNumberOperands(Token @operator, object left, object right)
        {
            if (TryCheckOperands<double>(left, right, out var result)) return result;
            throw new RuntimeErrorException(@operator, "Operands must be numbers.");
        }
        
        private bool TryCheckOperands<T>(object left, object right, out (T left, T right) result)
        {
            if (left is T l && right is T r)
            {
                result = (l, r);
                return true;
            }

            result = (default(T), default(T));
            return false;
        }
    }

    public class ReturnException : Exception
    {
        public object Value { get; }

        public ReturnException(object value)
        {
            Value = value;
        }
    }

    public sealed class Void
    {
        public static readonly Void Instance = new Void();
        private Void() {}
    }
}