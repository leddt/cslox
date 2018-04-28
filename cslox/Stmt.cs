namespace cslox {
  using System.Collections.Generic;

  [System.CodeDom.Compiler.GeneratedCode("cslox.GenerateAst", "0.0.0")]
  public abstract class Stmt {
    public abstract T Accept<T>(IStmtVisitor<T> visitor);

    public interface IStmtVisitor<T> {
      T Visit(Block stmt);
      T Visit(Expression stmt);
      T Visit(Print stmt);
      T Visit(Var stmt);
    }

    public class Block : Stmt {
      public List<Stmt> Statements { get; }

      public Block(List<Stmt> statements) {
        Statements = statements;
      }

      public override T Accept<T>(IStmtVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Expression : Stmt {
      public Expr Expr { get; }

      public Expression(Expr expr) {
        Expr = expr;
      }

      public override T Accept<T>(IStmtVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Print : Stmt {
      public Expr Expr { get; }

      public Print(Expr expr) {
        Expr = expr;
      }

      public override T Accept<T>(IStmtVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Var : Stmt {
      public Token Name { get; }
      public Expr Initializer { get; }

      public Var(Token name, Expr initializer) {
        Name = name;
        Initializer = initializer;
      }

      public override T Accept<T>(IStmtVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
  }
}
