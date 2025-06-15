using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public abstract class AbstractAST (string symbol,int line,int column): IAST
    {
        public int Line => line;

        public int Column => column;

        public string Symbol => _symbol;
        private string _symbol = symbol;

        public abstract object Eval(IContext context, IErrorColector colector);

        public void UpdateSymbol(string symbol)
        {
            _symbol = symbol;
        }
    }
}
