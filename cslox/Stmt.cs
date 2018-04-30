namespace cslox {
  [System.CodeDom.Compiler.GeneratedCode("cslox.GenerateAst", "0.0.0")]
  public abstract class Stmt {
    public abstract T Accept<T>(IStmtVisitor<T> visitor);

    public interface IStmtVisitor<T> {
      T Visit(Block stmt);
      T Visit(Class stmt);
      T Visit(Expression stmt);
      T Visit(Function stmt);
      T Visit(If stmt);
      T Visit(Print stmt);
      T Visit(Return stmt);
      T Visit(Var stmt);
      T Visit(While stmt);
    }

    public class Block : Stmt {
      public Stmt[] Statements { get; }

      public Block(Stmt[] statements) {
        Statements = statements;
      }

      public override T Accept<T>(IStmtVisitor<T> visitor) {
        return visitor.Visit(this);
      }
    }

    public class Class : Stmt {
      public Token Name { get; }
      public Expr.Variable Superclass { get; }
      public Stmt.Function[] Methods { get; }

      public Class(Token name, Expr.Variable superclass, Stmt.Function[] methods) {
        Name = name;
        Superclass = superclass;
        Methods = methods;
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
      public Token[] Parameters { get; }
      public Stmt[] Body { get; }

      public Function(Token name, Token[] parameters, Stmt[] body) {
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
