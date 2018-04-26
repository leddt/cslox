using System.Text;

namespace cslox
{
    public class AstPrinter : Expr.Visitor<string>
    {
        public string Print(Expr expr) => expr.Accept(this);
        

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value == null 
                ? "nil" 
                : expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Right);
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