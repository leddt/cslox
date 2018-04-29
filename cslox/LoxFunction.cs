using System.Collections.Generic;

namespace cslox
{
    public class LoxFunction : LoxCallable
    {
        private readonly Stmt.Function declaration;
        private readonly Environment closure;

        public LoxFunction(Stmt.Function declaration, Environment closure)
        {
            this.declaration = declaration;
            this.closure = closure;
        }

        public int Arity => declaration.Parameters.Length;

        public object Call(Interpreter interpreter, object[] arguments)
        {
            var environment = new Environment(closure);

            for (var i = 0; i < declaration.Parameters.Length; i++)
            {
                environment.Define(declaration.Parameters[i], arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            }
            catch (ReturnException returnValue)
            {
                return returnValue.Value;
            }

            return null;
        }

        public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
    }
}