using System.Collections.Generic;

namespace cslox
{
    public class LoxInstance
    {
        private readonly LoxClass klass;
        private readonly Dictionary<string, object> fields = new Dictionary<string, object>();

        public LoxInstance(LoxClass klass)
        {
            this.klass = klass;
        }

        public object Get(Token name)
        {
            if (fields.ContainsKey(name.Lexeme))
                return fields[name.Lexeme];

            var method = klass.FindMethod(this, name.Lexeme);
            if (method != null) return method;

            throw new RuntimeErrorException(name, $"Undefined property '{name.Lexeme}'.");
        }

        public void Set(Token name, object value)
        {
            fields[name.Lexeme] = value;
        }

        public override string ToString() => $"{klass.Name} instance";
    }
}