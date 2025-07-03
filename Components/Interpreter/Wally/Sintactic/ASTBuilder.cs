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
                try
                {
                    var x = Convert.ToInt32(a);
                    var y = Convert.ToInt32(b);
                    return x + y;
                }
                catch
                {
                    var e = Convert.ToString(a);
                    var d = Convert.ToString(b);
                    return e + d;
                }
            };
            BinaryOperators["-"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToInt32(a) - Convert.ToInt32(b);
                }
                catch
                {
                    colector.AddError(new Error($"Its not possible rest this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["*"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToInt32(a) * Convert.ToInt32(b);
                }catch
                {
                    colector.AddError(new Error($"Its not possible multiply this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["/"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToInt32(a) / Convert.ToInt32(b);
                }
                catch
                {
                    colector.AddError(new Error($"Its not possible divide this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["%"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToInt32(a) % Convert.ToInt32(b);
                }
                catch
                {
                    colector.AddError(new Error($"Its not possible divide this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["**"] = (l, r, context, colector) =>
            {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Math.Pow(Convert.ToInt32(a), Convert.ToInt32(b));
                }catch
                {
                    colector.AddError(new Error($"Its not possible power this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["||"] = (l, r, context, colector) => {
                var a =l.Eval(context, colector);
                var b =r.Eval(context, colector);
                try
                {
                    return Convert.ToBoolean(a) || Convert.ToBoolean(b);
                }catch
                {
                    colector.AddError(new Error($"Any of this elements its not bool{a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["&&"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToBoolean(a) && Convert.ToBoolean(b);
                }
                catch
                {
                    colector.AddError(new Error($"Any of this elements its not bool {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["=="] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToString(a) == Convert.ToString(b);
                }catch
                {
                    colector.AddError(new Error($"Its not possible Equal this elements {a} , {b}", r.Line, r.Column, ErrorType.Semantic));
                    return null;
                }
            };
            BinaryOperators["<"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToInt32(a) < Convert.ToInt32(b);
                }
                catch
                {
                    return Convert.ToString(a).Length < Convert.ToString(b).Length;
                }
            };
            BinaryOperators[">"] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToInt32(a) > Convert.ToInt32(b);
                }
                catch
                {
                    return Convert.ToString(a).Length > Convert.ToString(b).Length;
                }
            };
            BinaryOperators["<="] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToInt32(a) <= Convert.ToInt32(b);
                }
                catch
                {
                    return Convert.ToString(a).Length <= Convert.ToString(b).Length;
                }
            };
            BinaryOperators[">="] = (l, r, context, colector) => {
                var a = l.Eval(context, colector);
                var b = r.Eval(context, colector);
                try
                {
                    return Convert.ToInt32(a) >= Convert.ToInt32(b);
                }
                catch
                {
                    return Convert.ToString(a).Length >= Convert.ToString(b).Length;
                }
            };
            UnaryOperators["!"] = (target, context, colector) =>
            {
                var a = target.Eval(context,colector);
                try
                {
                    return !Convert.ToBoolean(a);
                }catch
                {
                    colector.AddError(new Error($"Its not possible negate this elements {a}" , target.Line, target.Column, ErrorType.Semantic));
                    return null;
                }
            };
            UnaryOperators["-"] = (target, context, colector) =>
            {
                var a = target.Eval(context, colector);
                try
                {
                    return -1 * Convert.ToInt32(a);
                }catch
                {
                    colector.AddError(new Error($"Its not possible negative this elements {a}", target.Line, target.Column, ErrorType.Semantic));
                    return null;
                }
            };
        }
        public IAST Build(IToken token,string endmarker)
        {
            if (token.Type() == Tokentype.EOF)
                return new AtomicAST(token.Lexeme(), token.Line(), token.Column(), token.Lexeme());
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
                    Tokentype.Operator => raw,
                };
            }catch
            {
                return new GarbageAST("GARBAGE", token.Line(), token.Column(), token.Lexeme());
            }
            return new AtomicAST(
                sym,
                token.Line(),
                token.Column(),
                token.Lexeme());
        }
    }
}
