namespace WallyInterpreter.Components.Interpreter.Tokens
{
    public interface IExtractorToken
    {
        IToken GetToken(Tokentype[] tokentypes, string text, int line, int column);
    }
}
