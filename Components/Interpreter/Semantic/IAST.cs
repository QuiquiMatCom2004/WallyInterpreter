using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public interface IAST
    {
        int Line { get; }
        int Column { get; } 
        string Symbol { get; }
        void UpdateSymbol(string symbol);
        object Eval(IContext context,IErrorColector colector);
    }
}
