﻿using System.Collections.Generic;

namespace cslox
{
    public class Environment
    {
        public Environment Enclosing { get; }

        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public Environment() {}
        public Environment(Environment enclosing)
        {
            Enclosing = enclosing;
        }

        public void Define(Token name, object value) => Define(name.Lexeme, value);
        public void Define(string name, object value) => values[name] = value;

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
                return values[name.Lexeme];

            if (Enclosing != null)
                return Enclosing.Get(name);

            throw new RuntimeErrorException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (Enclosing != null)
            {
                Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeErrorException(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}