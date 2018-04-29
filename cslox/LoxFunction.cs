using System.Collections.Generic;

namespace cslox
{
    public class LoxFunction : LoxCallable
    {
        private readonly Stmt.Function declaration;
        private readonly Environment closure;
        private readonly bool isInitializer;

        public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
        {
            this.declaration = declaration;
            this.closure = closure;
            this.isInitializer = isInitializer;
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

            if (isInitializer) return closure.GetAt(0, "this");
            return null;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            var env = new Environment(closure);
            env.Define("this", instance);

            return new LoxFunction(declaration, env, isInitializer);
        }

        public override string ToString() => $"<fn {declaration.Name.Lexeme}>";
    }
}