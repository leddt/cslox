using System;
using static cslox.Expr;
using static cslox.TokenType;

namespace cslox
{
    public class Interpreter : IExprVisitor<object>
    {
        public void Interpret(Expr expression)
        {
            try
            {
                var value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeErrorException error)
            {
                Lox.RuntimeError(error);
            }
        }

        public object Visit(Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case Minus:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double) left - (double) right;
                case Slash:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double) left / (double) right;
                case Star:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double) left * (double) right;

                case Plus:
                    if (left is double ld && right is double rd)
                        return ld + rd;

                    if (left is string ls && right is string rs)
                        return string.Concat(ls, rs);

                    throw new RuntimeErrorException(expr.Op, "Operands must be two numbers or two strings.");

                case Greater:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double) left >  (double) right;
                case GreaterEqual:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double) left >= (double) right;
                case Less:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double) left <  (double) right;
                case LessEqual:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double) left <= (double) right;

                case EqualEqual: return IsEqual(left, right);
                case BangEqual: return !IsEqual(left, right);
            }

            return null;
        }

        public object Visit(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object Visit(Literal expr)
        {
            return expr.Value;
        }

        public object Visit(Unary expr)
        {
            var right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case Minus:
                    CheckNumberOperand(expr.Op, right);
                    return -(double) right;
                case Bang:
                    return !IsTruthy(right);
            }

            return null;
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

        private void CheckNumberOperand(Token @operator, object operand)
        {
            if (operand is double) return;
            throw new RuntimeErrorException(@operator, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token @operator, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeErrorException(@operator, "Operands must be numbers.");
        }
    }
}