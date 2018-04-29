using System.Collections.Generic;

namespace cslox
{
    public class LoxClass : LoxCallable
    {
        private readonly Dictionary<string, LoxFunction> methods;

        public string Name { get; }
        public int Arity => methods.ContainsKey("init") ? methods["init"].Arity : 0;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            this.methods = methods;
            Name = name;
        }

        public object Call(Interpreter interpreter, object[] arguments)
        {
            var instance = new LoxInstance(this);

            FindMethod(instance, "init")
                ?.Call(interpreter, arguments);

            return instance;
        }

        public LoxFunction FindMethod(LoxInstance instance, string name)
        {
            return methods.ContainsKey(name) 
                ? methods[name].Bind(instance) 
                : null;
        }

        public override string ToString() => Name;
    }
}