using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class GarbageAST : AbstractAST
    {
        private object _value;
        public GarbageAST(string symbol, int line, int column, object value) : base(symbol, line, column)
        {
            _value = value;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            return _value;
        }
    }
}
