using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace VSNL.Core
{
    public static class ExpressionEvaluator
    {
        private enum TokenType { Number, String, Operator, OpenParen, CloseParen, Boolean }
        private struct Token
        {
            public TokenType Type;
            public string Value;
        }

        public static object Evaluate(string expression)
        {
            try
            {
                var tokens = Tokenize(expression);
                var rpn = ShuntingYard(tokens);
                return EvaluateRPN(rpn);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ExpressionEvaluator] Error evaluating '{expression}': {ex.Message}");
                return 0f;
            }
        }

        private static List<Token> Tokenize(string expr)
        {
            var tokens = new List<Token>();
            int i = 0;
            while (i < expr.Length)
            {
                char c = expr[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (c == '(') { tokens.Add(new Token { Type = TokenType.OpenParen, Value = "(" }); i++; }
                else if (c == ')') { tokens.Add(new Token { Type = TokenType.CloseParen, Value = ")" }); i++; }
                else if (char.IsDigit(c) || c == '.')
                {
                    // Number
                    int start = i;
                    while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.')) i++;
                    tokens.Add(new Token { Type = TokenType.Number, Value = expr.Substring(start, i - start) });
                }
                else if (c == '"')
                {
                    // String
                    i++; // skip open quote
                    int start = i;
                    while (i < expr.Length && expr[i] != '"') i++;
                    tokens.Add(new Token { Type = TokenType.String, Value = expr.Substring(start, i - start) });
                    i++; // skip close quote
                }
                else if (IsOperatorChar(c))
                {
                    // Operator (greedy match for >=, ==, &&, etc)
                    int start = i;
                    while (i < expr.Length && IsOperatorChar(expr[i])) i++;
                    string op = expr.Substring(start, i - start);
                    tokens.Add(new Token { Type = TokenType.Operator, Value = op });
                }
                else if (char.IsLetter(c))
                {
                    // Boolean keywords or unquoted strings (true/false)
                    int start = i;
                    while (i < expr.Length && char.IsLetterOrDigit(expr[i])) i++;
                    string word = expr.Substring(start, i - start).ToLower();
                    if (word == "true" || word == "false")
                        tokens.Add(new Token { Type = TokenType.Boolean, Value = word });
                    else
                         // Treat unquoted text as string (or variable name if not substituted, but we assume vars are substituted already)
                        tokens.Add(new Token { Type = TokenType.String, Value = word }); 
                }
                else
                {
                    i++;
                }
            }
            return tokens;
        }

        private static bool IsOperatorChar(char c) => "+-*/%&|!=<>".IndexOf(c) >= 0;

        private static Queue<Token> ShuntingYard(List<Token> tokens)
        {
            var output = new Queue<Token>();
            var ops = new Stack<Token>();

            foreach (var t in tokens)
            {
                switch (t.Type)
                {
                    case TokenType.Number:
                    case TokenType.String:
                    case TokenType.Boolean:
                        output.Enqueue(t);
                        break;
                    case TokenType.Operator:
                        while (ops.Count > 0 && ops.Peek().Type == TokenType.Operator &&
                               Precedence(ops.Peek().Value) >= Precedence(t.Value))
                        {
                            output.Enqueue(ops.Pop());
                        }
                        ops.Push(t);
                        break;
                    case TokenType.OpenParen:
                        ops.Push(t);
                        break;
                    case TokenType.CloseParen:
                        while (ops.Count > 0 && ops.Peek().Type != TokenType.OpenParen)
                        {
                            output.Enqueue(ops.Pop());
                        }
                        if (ops.Count > 0) ops.Pop(); // Pop open paren
                        break;
                }
            }
            while (ops.Count > 0) output.Enqueue(ops.Pop());
            return output;
        }

        private static int Precedence(string op)
        {
            if (op == "!" || op == "u-") return 9; // Unary (not supported fully here yet)
            if (op == "*" || op == "/" || op == "%") return 8;
            if (op == "+" || op == "-") return 7;
            if (op == "<" || op == ">" || op == "<=" || op == ">=") return 6;
            if (op == "==" || op == "!=") return 5;
            if (op == "&&") return 4;
            if (op == "||") return 3;
            return 0;
        }

        private static object EvaluateRPN(Queue<Token> rpn)
        {
            var stack = new Stack<object>();

            while (rpn.Count > 0)
            {
                var t = rpn.Dequeue();
                if (t.Type == TokenType.Number) stack.Push(float.Parse(t.Value, CultureInfo.InvariantCulture));
                else if (t.Type == TokenType.String) stack.Push(t.Value);
                else if (t.Type == TokenType.Boolean) stack.Push(bool.Parse(t.Value));
                else if (t.Type == TokenType.Operator)
                {
                    if (stack.Count < 2) return 0f; // Error
                    var b = stack.Pop();
                    var a = stack.Pop();
                    stack.Push(ApplyOp(t.Value, a, b));
                }
            }

            return stack.Count > 0 ? stack.Pop() : 0f;
        }

        private static object ApplyOp(string op, object a, object b)
        {
            // Simple type coercion: try float first
            float fa = 0, fb = 0;
            bool isNumA = TryParseFloat(a, out fa);
            bool isNumB = TryParseFloat(b, out fb);

            if (isNumA && isNumB)
            {
                switch (op)
                {
                    case "+": return fa + fb;
                    case "-": return fa - fb;
                    case "*": return fa * fb;
                    case "/": return fb != 0 ? fa / fb : 0;
                    case "%": return fa % fb;
                    case ">": return fa > fb;
                    case "<": return fa < fb;
                    case ">=": return fa >= fb;
                    case "<=": return fa <= fb;
                    case "==": return Math.Abs(fa - fb) < 0.0001f;
                    case "!=": return Math.Abs(fa - fb) > 0.0001f;
                }
            }

            // Boolean Logic
            bool boolA = a is bool ? (bool)a : false;
            bool boolB = b is bool ? (bool)b : false;
             
            // Coerce numbers to bool for logic? (0 = false, else true)
            if (isNumA) boolA = fa != 0;
            if (isNumB) boolB = fb != 0;

            switch (op)
            {
                case "&&": return boolA && boolB;
                case "||": return boolA || boolB;
            }

            // String Logic
            string sa = a.ToString();
            string sb = b.ToString();
            switch (op)
            {
                case "+": return sa + sb; // Concatenation
                case "==": return sa == sb;
                case "!=": return sa != sb;
            }

            return 0f;
        }

        private static bool TryParseFloat(object obj, out float result)
        {
            if (obj is float f) { result = f; return true; }
            if (obj is int i) { result = i; return true; }
            if (obj is string s) return float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            result = 0;
            return false;
        }
    }
}
