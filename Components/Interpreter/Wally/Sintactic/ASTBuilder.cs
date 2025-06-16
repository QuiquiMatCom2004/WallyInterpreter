using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.Wally.Sintactic
{
    public class ASTBuilder
    {
        public Dictionary<string, Func<IAST, IAST, IContext, IErrorColector, object>> BinaryOperators = new Dictionary<string, Func<IAST, IAST, IContext, IErrorColector, object>>();
        public Dictionary<string, Func<IAST, IContext, IErrorColector, object>> UnaryOperators = new Dictionary<string, Func<IAST, IContext, IErrorColector, object>>();

        public ASTBuilder()
        {

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
            var raw = token.Lexeme().Trim();
            string sym;
            try
            {
                sym = token.Type() switch
                {
                    Tokentype.Identifier => "id",
                    Tokentype.Number => "number",
                    Tokentype.Boolean => "boolean",
                    Tokentype.String => "string",
                    Tokentype.EOL => "EOL",
                    Tokentype.Keyword => raw,
                    Tokentype.Symbol => raw,
                    Tokentype.Operator => raw
                };
            }catch
            {
                return new GarbageAST(token.Lexeme(), token.Line(), token.Column(), token);
            }
            return new AtomicAST(
                sym,
                token.Line(),
                token.Column(),
                token.Lexeme());
        }
    }
}
