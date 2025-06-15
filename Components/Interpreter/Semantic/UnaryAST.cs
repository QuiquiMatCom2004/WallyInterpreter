using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class UnaryAST : AbstractAST
    {
        private Func<IAST, IContext, IErrorColector, object> _operator;
        public IAST target;
        public UnaryAST(string symbol, int line, int column,Func<IAST,IContext,IErrorColector,object> op ) : base(symbol, line, column)
        {
            _operator = op;
            this.target = null ;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            return _operator(target, context, colector);
        }
    }
}
