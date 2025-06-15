namespace WallyInterpreter.Components.Interpreter.Errors
{
    public interface IErrorColector
    {
        IError[] GetErrors();
        void AddError(IError error);
    }
}
