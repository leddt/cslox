﻿using System.Collections.Generic;
using System.Linq;

namespace cslox
{
    public class Resolver : Expr.IExprVisitor<Void>, Stmt.IStmtVisitor<Void>
    {
        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.None;
        private ClassType currentClass = ClassType.None;

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public Void Visit(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);

            return Void.Instance;
        }

        public Void Visit(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);

            return Void.Instance;
        }

        public Void Visit(Expr.Call expr)
        {
            Resolve(expr.Callee);

            foreach (var arg in expr.Arguments)
                Resolve(arg);

            return Void.Instance;
        }

        public Void Visit(Expr.Get expr)
        {
            Resolve(expr.Obj);
            return Void.Instance;
        }

        public Void Visit(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return Void.Instance;
        }

        public Void Visit(Expr.Literal expr)
        {
            return Void.Instance;
        }

        public Void Visit(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);

            return Void.Instance;
        }

        public Void Visit(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Obj);

            return Void.Instance;
        }

        public Void Visit(Expr.Super expr)
        {
            if (currentClass == ClassType.None)
                Lox.Error(expr.Keyword, "Cannot use 'super' outside of a class.");
            else if (currentClass != ClassType.Subclass)
                Lox.Error(expr.Keyword, "Cannot use 'super' in a class with no superclass.");

            ResolveLocal(expr, expr.Keyword);
            return Void.Instance;
        }

        public Void Visit(Expr.This expr)
        {
            if (currentClass == ClassType.None)
                Lox.Error(expr.Keyword, "Cannot use 'this' outside of a class.");
            else
                ResolveLocal(expr, expr.Keyword);

            return Void.Instance;
        }

        public Void Visit(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return Void.Instance;
        }

        public Void Visit(Expr.Variable expr)
        {
            if (scopes.Any())
            {
                var scope = scopes.Peek();
                var name = expr.Name.Lexeme;

                if (scope.ContainsKey(name) && scope[name] == false) 
                    Lox.Error(expr.Name, "Cannot read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);

            return Void.Instance;
        }

        public Void Visit(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();

            return Void.Instance;
        }

        public Void Visit(Stmt.Class stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            var enclosingClass = currentClass;
            currentClass = ClassType.Class;

            if (stmt.Superclass != null)
            {
                currentClass = ClassType.Subclass;
                Resolve(stmt.Superclass);

                BeginScope();
                scopes.Peek()["super"] = true;
            }

            BeginScope();
            scopes.Peek()["this"] = true;

            foreach (var method in stmt.Methods)
            {
                var decl = method.Name.Lexeme.Equals("init")
                    ? FunctionType.Initializer
                    : FunctionType.Method;

                ResolveFunction(method, decl);
            }

            EndScope();
            if (stmt.Superclass != null) EndScope();

            currentClass = enclosingClass;

            return Void.Instance;
        }

        public Void Visit(Stmt.Expression stmt)
        {
            Resolve(stmt.Expr);
            return Void.Instance;
        }

        public Void Visit(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            ResolveFunction(stmt, FunctionType.Function);

            return Void.Instance;
        }

        public Void Visit(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);

            return Void.Instance;
        }

        public Void Visit(Stmt.Print stmt)
        {
            Resolve(stmt.Expr);
            return Void.Instance;
        }

        public Void Visit(Stmt.Return stmt)
        {
            if (currentFunction == FunctionType.None)
                Lox.Error(stmt.Keyword, "Cannot return from top-level code.");

            if (stmt.Value != null)
            {
                if (currentFunction == FunctionType.Initializer)
                    Lox.Error(stmt.Keyword, "Cannot return a value from an initializer.");

                Resolve(stmt.Value);
            }

            return Void.Instance;
        }

        public Void Visit(Stmt.Var stmt)
        {
            Declare(stmt.Name);

            if (stmt.Initializer != null)
                Resolve(stmt.Initializer);

            Define(stmt.Name);

            return Void.Instance;
        }

        public Void Visit(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);

            return Void.Instance;
        }



        public void Resolve(Stmt[] statements)
        {
            foreach (var stmt in statements)
                Resolve(stmt);
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (var i = 0; i < scopes.Count; i++)
            {
                if (scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, i);
                    return;
                }
            }
        }

        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            var enclosingFunction = currentFunction;
            currentFunction = type;

            BeginScope();
            
            foreach (var param in function.Parameters)
            {
                Declare(param);
                Define(param);
            }

            Resolve(function.Body);

            EndScope();

            currentFunction = enclosingFunction;
        }

        private void BeginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            scopes.Pop();
        }

        private void Declare(Token name)
        {
            if (scopes.Count == 0) return;

            var scope = scopes.Peek();
            
            if (scope.ContainsKey(name.Lexeme))
                Lox.Error(name, "Variable with this name already declared in this scope.");

            scope[name.Lexeme] = false;
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.Lexeme] = true;
        }

        private enum FunctionType
        {
            None,
            Function,
            Method,
            Initializer
        }

        private enum ClassType
        {
            None,
            Class,
            Subclass
        }
    }

}