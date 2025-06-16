using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Parser;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Draw
{
    public static class Information
    {
        public static List<IToken> tokens = new List<IToken>();
        public static List<IAST> asts = new List<IAST>();
        public static Queue<ParserAction> actions = new Queue<ParserAction>();
        public static List<IError> errors = new List<IError>();
    }
}
