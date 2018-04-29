using System;
using System.Collections.Generic;
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
                "Assign   : Token name, Expr value",
                "Binary   : Expr left, Token op, Expr right",
                "Call     : Expr callee, Token paren, Expr[] arguments",
                "Grouping : Expr expression",
                "Literal  : object value",
                "Logical  : Expr left, Token op, Expr right",
                "Unary    : Token op, Expr right",
                "Variable : Token name"
            );

            DefineAst(outputDir, "Stmt",
                "Block      : Stmt[] statements",
                "Expression : Expr expr",
                "Function   : Token name, Token[] parameters, Stmt[] body",
                "If         : Expr condition, Stmt thenBranch, Stmt elseBranch",
                "Print      : Expr expr",
                "Return     : Token keyword, Expr value",
                "Var        : Token name, Expr initializer",
                "While      : Expr condition, Stmt body"
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

                writer.WriteLine($"    public abstract T Accept<T>(I{baseName}Visitor<T> visitor);");

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

        private static void DefineVisitor(TextWriter writer, string baseName, string[] types)
        {
            writer.WriteLine();
            writer.WriteLine($"    public interface I{baseName}Visitor<T> {{");

            foreach (var type in types)
            {
                var name = type.Split(':')[0].Trim();
                writer.WriteLine($"      T Visit({name} {baseName.ToLower()});");
            }

            writer.WriteLine($"    }}");
        }

        private static void DefineType(TextWriter writer, string baseName, string className, string fieldList)
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
            writer.WriteLine($"      public override T Accept<T>(I{baseName}Visitor<T> visitor) {{");
            writer.WriteLine($"        return visitor.Visit(this);");
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