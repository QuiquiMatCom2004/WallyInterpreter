using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.LexicalAnalizer
{
    public interface ILexicalAnalizer
    {
        IError CheckRule(IToken token);

        void AddRule(Tokentype token, ILexicalRule rule);
    }
}
