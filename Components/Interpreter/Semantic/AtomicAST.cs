using System.Linq.Expressions;
using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class AtomicAST : AbstractAST
    {
        private object _value;
        public AtomicAST(string symbol, int line, int column,object value) : base(symbol, line, column)
        {
            _value = value;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            return _value;
        }
    }
}
