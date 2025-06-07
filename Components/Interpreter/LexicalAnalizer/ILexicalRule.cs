using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.LexicalAnalizer
{
    public interface ILexicalRule
    {
        string ErrorRule();
        Func<IToken, bool> Rule();
    }
}
