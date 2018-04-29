namespace cslox {
  using System.Collections.Generic;

  [System.CodeDom.Compiler.GeneratedCode("cslox.GenerateAst", "0.0.0")]
  public abstract class Stmt {
    public abstract T Accept<T>(IStmtVisitor<T> visitor);

    public interface IStmtVisitor<T> {
      T Visit(Block stmt);
      T Visit(Expression stmt);
      T Visit(Function stmt);
      T Visit(If stmt);
      T Visit(Print stmt);
      T Visit(Return stmt);
      T Visit(Var stmt);
      T Visit(While stmt);
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

    public class Function : Stmt {
      public Token Name { get; }
      public List<Token> Parameters { get; }
      public List<Stmt> Body { get; }

      public Function(Token name, List<Token> parameters, List<Stmt> body) {
        Name = name;
        Parameters = parameters;
        Body = body;
      }

      public override T Accept<T>(IStmtVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class If : Stmt {
      public Expr Condition { get; }
      public Stmt ThenBranch { get; }
      public Stmt ElseBranch { get; }

      public If(Expr condition, Stmt thenBranch, Stmt elseBranch) {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
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

    public class Return : Stmt {
      public Token Keyword { get; }
      public Expr Value { get; }

      public Return(Token keyword, Expr value) {
        Keyword = keyword;
        Value = value;
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

    public class While : Stmt {
      public Expr Condition { get; }
      public Stmt Body { get; }

      public While(Expr condition, Stmt body) {
        Condition = condition;
        Body = body;
      }

      public override T Accept<T>(IStmtVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }
  }
}
