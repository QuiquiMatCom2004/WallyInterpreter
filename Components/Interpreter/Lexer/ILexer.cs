using WallyInterpreter.Components.Interpreter.Granmar;
using WallyInterpreter.Components.Interpreter.Tokens;
using WallyInterpreter.Components.Interpreter.Tools;

namespace WallyInterpreter.Components.Interpreter.Lexer
{
    public interface ILexer : IGenerator<IToken>
    {
        void LoadCode(string code);
        void AddTokenExpresion(Tokentype type, int priority, IGranmar re);
    }
}
