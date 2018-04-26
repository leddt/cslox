namespace cslox {
  [System.CodeDom.Compiler.GeneratedCode("cslox.GenerateAst", "0.0.0")]
  public abstract class Expr {
    public abstract T Accept<T>(IExprVisitor<T> visitor);

    public interface IExprVisitor<T> {
      T Visit(Binary expr);
      T Visit(Grouping expr);
      T Visit(Literal expr);
      T Visit(Unary expr);
    }

    public class Binary : Expr {
      public Expr Left { get; }
      public Token Op { get; }
      public Expr Right { get; }

      public Binary(Expr left, Token op, Expr right) {
        Left = left;
        Op = op;
        Right = right;
      }

      public override T Accept<T>(IExprVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Grouping : Expr {
      public Expr Expression { get; }

      public Grouping(Expr expression) {
        Expression = expression;
      }

      public override T Accept<T>(IExprVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Literal : Expr {
      public object Value { get; }

      public Literal(object value) {
        Value = value;
      }

      public override T Accept<T>(IExprVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Unary : Expr {
      public Token Op { get; }
      public Expr Right { get; }

      public Unary(Token op, Expr right) {
        Op = op;
        Right = right;
      }

      public override T Accept<T>(IExprVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
  }
}
