using System;
using System.Collections.Generic;

namespace cslox
{
    public class NativeFunction : LoxCallable
    {
        private readonly Func<Interpreter, List<object>, object> function;

        public NativeFunction(int arity, Func<Interpreter, List<object>, object> function)
        {
            this.function = function;
            Arity = arity;
        }

        public int Arity { get; }
        public object Call(Interpreter interpreter, List<object> arguments) => function(interpreter, arguments);
    }
}