using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.LexicalAnalizer
{
    public interface ILexicalAnalizer
    {
        void CheckRule(IToken token);

        void AddRule(Tokentype token, ILexicalRule rule);
    }
}
