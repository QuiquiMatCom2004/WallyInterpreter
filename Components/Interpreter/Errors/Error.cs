namespace WallyInterpreter.Components.Interpreter.Errors
{
    public class Error (string message,int line, int column,ErrorType type): IError
    {
        public string Message => message;

        public int Line => line;

        public int Column => column;
         
        public ErrorType Type => type;
    }
}
