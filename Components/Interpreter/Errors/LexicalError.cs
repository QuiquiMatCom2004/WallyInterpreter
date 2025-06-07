namespace WallyInterpreter.Components.Interpreter.Errors
{
    public class LexicalError: Exception
    {
        public LexicalError(string message) : base(message) { }
    }
}
