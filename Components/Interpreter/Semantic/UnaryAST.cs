using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class UnaryAST : AbstractAST
    {
        private Func<IAST, IContext, IErrorColector, object> _operator;
        IAST target;
        public UnaryAST(string symbol, int line, int column,Func<IAST,IContext,IErrorColector,object> op, IAST target ) : base(symbol, line, column)
        {
            _operator = op;
            this.target = target ;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            return _operator(target, context, colector);
        }
    }
}
