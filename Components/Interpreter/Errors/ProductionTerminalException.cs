namespace WallyInterpreter.Components.Interpreter.Errors
{
    public class ProductionError:Exception
    {
        public ProductionError() { }
        public ProductionError(string message) : base(message) { }
    }
}
