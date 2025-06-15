using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.Wally.Sintactic
{
    public class ASTBuilder
    {
        Dictionary<Tokentype,string> symbolsByToken = new Dictionary<Tokentype,string>();
        Dictionary<string, Func<IAST, IAST, IContext, IErrorColector, object>> BinaryOperators = new Dictionary<string, Func<IAST, IAST, IContext, IErrorColector, object>>();
        Dictionary<string, Func<IAST, IContext, IErrorColector, object>> UnaryOperators = new Dictionary<string, Func<IAST, IContext, IErrorColector, object>>();

        public ASTBuilder()
        {
            symbolsByToken[Tokentype.Number] = "number";
            symbolsByToken[Tokentype.String] = "string";
            symbolsByToken[Tokentype.Boolean] = "boolean";
            symbolsByToken[Tokentype.Identifier] = "id";


            BinaryOperators["+"] = (l,r,context,colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is string sa && b is string sb)
                    return sa + sb;
                else if (a is int ia && b is int ib)
                    return ia + ib;
                else if (a is float fa && b is float fb)
                    return fa + fb;
                else
                {
                    colector.AddError(new Error($"Its not possible sum this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["-"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is int ia && b is int ib)
                    return ia - ib;
                else if (a is float fa && b is float fb)
                    return fa - fb;
                else
                {
                    colector.AddError(new Error($"Its not possible rest this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["*"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is int ia && b is int ib)
                    return ia * ib;
                else if (a is float fa && b is float fb)
                    return fa * fb;
                else
                {
                    colector.AddError(new Error($"Its not possible multiply this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["/"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is int ia && b is int ib)
                    return ia / ib;
                else if (a is float fa && b is float fb)
                    return fa / fb;
                else
                {
                    colector.AddError(new Error($"Its not possible divide this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["%"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is int ia && b is int ib)
                    return ia % ib;
                else if (a is float fa && b is float fb)
                    return fa % fb;
                else
                {
                    colector.AddError(new Error($"Its not possible divide this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["**"] = (l, r, context, colector) =>
            {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is int ia && b is int ib)
                    return Math.Pow(ia, ib);
                else if (a is float fa && b is float fb)
                    return Math.Pow(fa, fb);
                else
                {
                    colector.AddError(new Error($"Its not possible power this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["||"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is bool sa && b is bool sb)
                    return sa || sb;
                else
                {
                    colector.AddError(new Error($"Any of this elements its not bool{a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["&&"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is bool sa && b is bool sb)
                    return sa && sb;
                else
                {
                    colector.AddError(new Error($"Any of this elements its not bool {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["=="] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is bool ba && b is bool bb)
                    return ba == bb;
                else if (a is string sa && b is string sb)
                    return sa == sb;
                else if (a is int ia && b is int ib)
                    return ia == ib;
                else if (a is float fa && b is float fb)
                    return fa - fb < 0.00000001;
                else
                {
                    colector.AddError(new Error($"Its not possible Equal this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["<"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is string sa && b is string sb)
                    return sa.Length < sb.Length;
                else if (a is int ia && b is int ib)
                    return ia < ib;
                else if (a is float fa && b is float fb)
                    return fa <fb ;
                else
                {
                    colector.AddError(new Error($"Its not possible Minus this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators[">"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is string sa && b is string sb)
                    return sa.Length > sb.Length;
                else if (a is int ia && b is int ib)
                    return ia > ib;
                else if (a is float fa && b is float fb)
                    return fa > fb;
                else
                {
                    colector.AddError(new Error($"Its not possible Mayor this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["<="] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is string sa && b is string sb)
                    return sa.Length <= sb.Length;
                else if (a is int ia && b is int ib)
                    return ia <= ib;
                else if (a is float fa && b is float fb)
                    return fa <= fb;
                else
                {
                    colector.AddError(new Error($"Its not possible Minus or equal this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators[">="] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                if (a is string sa && b is string sb)
                    return sa.Length >= sb.Length;
                else if (a is int ia && b is int ib)
                    return ia >= ib;
                else if (a is float fa && b is float fb)
                    return fa >= fb;
                else
                {
                    colector.AddError(new Error($"Its not possible Mayor or equal this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            UnaryOperators["!"] = (target, context, colector) =>
            {
                var a = target.Eval(context,colector);
                if (a is bool sa)
                    return !sa;
                else
                {
                    colector.AddError(new Error($"Its not possible negate this elements {a}" , target.Line, target.Column, ErrorType.Semantic));
                    return null;
                }
            };
            UnaryOperators["-"] = (target, context, colector) =>
            {
                var a = target.Eval(context, colector);
                if (a is int sa)
                    return (-1)*sa;
                else if (a is float fa)
                    return (-1) * fa;
                else
                {
                    colector.AddError(new Error($"Its not possible negative this elements {a}", target.Line, target.Column, ErrorType.Semantic));
                    return null;
                }
            };
        }
        public IAST Build(IToken token,string endmarker)
        {
            switch(token.Type())
            {
                case Tokentype.Number:
                    var val = float.Parse(token.Lexeme());
                    return new AtomicAST(symbolsByToken[token.Type()], token.Line(), token.Column(), val);
                case Tokentype.Boolean:
                    var val1 = bool.Parse(token.Lexeme());
                    return new AtomicAST(symbolsByToken[token.Type()], token.Line(), token.Column(), val1);
                case Tokentype.String:
                    return new AtomicAST(symbolsByToken[token.Type()], token.Line(), token.Column(), token.Lexeme());
                case Tokentype.Identifier:
                    return new VariableAST(symbolsByToken[token.Type()],token.Line(),token.Column(),token.Lexeme());
                case Tokentype.Symbol:
                    if (token.Lexeme() == endmarker)
                        return new AtomicAST(endmarker, token.Line(), token.Column(), endmarker);
                    return new AtomicAST(token.Lexeme(), token.Line(), token.Column(), token.Lexeme());
                case Tokentype.Keyword:
                    switch (token.Lexeme())
                    {
                        case "Goto":
                            return new AtomicAST(token.Lexeme(),token.Line(),token.Column(),token.Lexeme());
                        default:
                            return new GarbageAST(token.Lexeme(),token.Line(),token.Column(),token.Lexeme());
                    }
                case Tokentype.Operator:
                    if (BinaryOperators.ContainsKey(token.Lexeme()))
                    {
                        return new BinaryAST(token.Lexeme(), token.Line(), token.Column(), BinaryOperators[token.Lexeme()]);
                    }
                    if (UnaryOperators.ContainsKey(token.Lexeme()))
                    {
                        return new UnaryAST(token.Lexeme(), token.Line(), token.Column(), UnaryOperators[token.Lexeme()]);
                    }
                    throw new Exception($"Operator {token.Lexeme()} not implemented");
                case Tokentype.EOL:
                    return new AtomicAST(token.Lexeme(), token.Line(), token.Column(), token.Lexeme()) ;
                default:
                    return new GarbageAST(token.Lexeme(),token.Line(),token.Column(), token.Lexeme());

            }
        }
    }
}
