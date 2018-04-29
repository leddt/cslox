using System.Text;

using static cslox.Expr;

namespace cslox
{
    public class AstPrinter : IExprVisitor<string>
    {
        public string Print(Expr expr) => expr.Accept(this);


        public string Visit(Assign expr)
        {
            return Parenthesize($"assign {expr.Name}", expr.Value);
        }

        public string Visit(Binary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        public string Visit(Call expr)
        {
            return Parenthesize(expr.Callee.Accept(this), expr.Arguments);
        }

        public string Visit(Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string Visit(Literal expr)
        {
            return expr.Value == null 
                ? "nil" 
                : expr.Value.ToString();
        }

        public string Visit(Logical expr)
        {
            return $"({expr.Left.Accept(this)} {expr.Op.Lexeme} {expr.Right.Accept(this)})";
        }

        public string Visit(Unary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Right);
        }

        public string Visit(Variable expr)
        {
            return Parenthesize($"variable {expr.Name}");
        }


        private string Parenthesize(string name, params Expr[] exprs)
        {
            var sb = new StringBuilder();

            sb.Append("(").Append(name);
            foreach (var expr in exprs)
                sb.Append(" ").Append(expr.Accept(this));
            sb.Append(")");

            return sb.ToString();
        }
    }
}