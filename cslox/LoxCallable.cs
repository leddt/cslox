using System.Collections.Generic;

namespace cslox
{
    public interface LoxCallable
    {
        int Arity { get; }
        object Call(Interpreter interpreter, object[] arguments);
    }
}