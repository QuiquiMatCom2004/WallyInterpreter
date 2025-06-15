namespace WallyInterpreter.Components.Interpreter.Errors
{
    public interface IError
    {
        string Message { get; }
        int Line {  get; }
        int Column { get; }
        ErrorType Type { get; }
    }
}
