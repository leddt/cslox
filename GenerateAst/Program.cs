﻿using System;
using System.IO;
using System.Linq;

namespace GenerateAst
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: GenerateAst <output directory>");
                Environment.Exit(1);
            }

            var outputDir = args[0];

            DefineAst(outputDir, "Expr",
                "Binary   : Expr left, Token op, Expr right",
                "Grouping : Expr expression",
                "Literal  : object value",
                "Unary    : Token op, Expr right"
            );
        }

        private static void DefineAst(string outputDir, string baseName, params string[] types)
        {
            var path = Path.Combine(outputDir, $"{baseName}.cs");

            using (var writer = File.CreateText(path))
            {
                writer.WriteLine("namespace cslox {");
                writer.WriteLine($"  [System.CodeDom.Compiler.GeneratedCode(\"cslox.GenerateAst\", \"0.0.0\")]");
                writer.WriteLine($"  public abstract class {baseName} {{");

                writer.WriteLine($"    public abstract T Accept<T>(Visitor<T> visitor);");

                DefineVisitor(writer, baseName, types);

                foreach (var type in types)
                {
                    var className = type.Split(':')[0].Trim();
                    var fields = type.Split(':')[1].Trim();
                    DefineType(writer, baseName, className, fields);
                }

                writer.WriteLine("  }");
                writer.WriteLine("}");
            }
        }

        private static void DefineVisitor(StreamWriter writer, string baseName, string[] types)
        {
            writer.WriteLine();
            writer.WriteLine("    public interface Visitor<T> {");

            foreach (var type in types)
            {
                var name = type.Split(':')[0].Trim();
                writer.WriteLine($"      T Visit{name}{baseName}({name} {baseName.ToLower()});");
            }

            writer.WriteLine("    }");
        }

        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            var fields = fieldList.Split(",")
                .Select(f => f.Trim())
                .Select(f => new {
                    type = f.Split(' ')[0],
                    name = f.Split(' ')[1]
                })
                .ToList();

            writer.WriteLine();
            writer.WriteLine($"    public class {className} : {baseName} {{");

            // Properties
            foreach (var field in fields)
                writer.WriteLine($"      public {field.type} {Capitalize(field.name)} {{ get; }}");

            writer.WriteLine();

            // Constructor
            writer.WriteLine($"      public {className}({fieldList}) {{");
            foreach (var field in fields)
                writer.WriteLine($"        {Capitalize(field.name)} = {field.name};");
            writer.WriteLine($"      }}");

            writer.WriteLine();

            // Visitor pattern
            writer.WriteLine($"      public override T Accept<T>(Visitor<T> visitor) {{");
            writer.WriteLine($"        return visitor.Visit{className}{baseName}(this);");
            writer.WriteLine($"      }}");

            writer.WriteLine($"    }}");
        }

        private static string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return input.First().ToString().ToUpper() + string.Join("", input.Skip(1));
        }
    }
}