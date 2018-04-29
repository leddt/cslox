namespace cslox {
  [System.CodeDom.Compiler.GeneratedCode("cslox.GenerateAst", "0.0.0")]
  public abstract class Expr {
    public abstract T Accept<T>(IExprVisitor<T> visitor);

    public interface IExprVisitor<T> {
      T Visit(Assign expr);
      T Visit(Binary expr);
      T Visit(Call expr);
      T Visit(Get expr);
      T Visit(Grouping expr);
      T Visit(Literal expr);
      T Visit(Logical expr);
      T Visit(Set expr);
      T Visit(This expr);
      T Visit(Unary expr);
      T Visit(Variable expr);
    }

    public class Assign : Expr {
      public Token Name { get; }
      public Expr Value { get; }

      public Assign(Token name, Expr value) {
        Name = name;
        Value = value;
      }

      public override T Accept<T>(IExprVisitor<T> visitor) {
        return visitor.Visit(this);
      }
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

    public class Call : Expr {
      public Expr Callee { get; }
      public Token Paren { get; }
      public Expr[] Arguments { get; }

      public Call(Expr callee, Token paren, Expr[] arguments) {
        Callee = callee;
        Paren = paren;
        Arguments = arguments;
      }

      public override T Accept<T>(IExprVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Get : Expr {
      public Expr Obj { get; }
      public Token Name { get; }

      public Get(Expr obj, Token name) {
        Obj = obj;
        Name = name;
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

    public class Logical : Expr {
      public Expr Left { get; }
      public Token Op { get; }
      public Expr Right { get; }

      public Logical(Expr left, Token op, Expr right) {
        Left = left;
        Op = op;
        Right = right;
      }

      public override T Accept<T>(IExprVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Set : Expr {
      public Expr Obj { get; }
      public Token Name { get; }
      public Expr Value { get; }

      public Set(Expr obj, Token name, Expr value) {
        Obj = obj;
        Name = name;
        Value = value;
      }

      public override T Accept<T>(IExprVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class This : Expr {
      public Token Keyword { get; }

      public This(Token keyword) {
        Keyword = keyword;
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

    public class Variable : Expr {
      public Token Name { get; }

      public Variable(Token name) {
        Name = name;
      }

      public override T Accept<T>(IExprVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
  }
}
