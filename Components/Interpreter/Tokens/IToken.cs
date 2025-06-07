namespace WallyInterpreter.Components.Interpreter.Tokens
{
    public interface IToken
    {
        int Column();
        int Line();
        string Lexeme();
        Tokentype Type();
    }
}
