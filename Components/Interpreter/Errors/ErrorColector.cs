using WallyInterpreter.Components.Services;

namespace WallyInterpreter.Components.Interpreter.Errors
{
    public class ErrorColector : IErrorColector
    {
        private List<IError> _errors = new List<IError>();
        private ConfigurationLog log = new();
        public void AddError(IError error)
        {
            _errors.Add(error);
            if(error.Type == ErrorType.Lexical)
                log.LogWarning(error.Message);
            if (error.Type == ErrorType.Semantic)
                log.LogInformation(error.Message);
            else log.LogError(new Exception(error.Message));
        }

        public IError[] GetErrors()
        {
            return _errors.ToArray();
        }
    }
}
