using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class BinaryAST : AbstractAST
    {
        private Func<IAST, IAST, IContext, IErrorColector, object> _operator;
        public IAST Left = null ;
        public IAST Right = null ;

        public BinaryAST(string symbol, int line, int column, Func<IAST, IAST, IContext, IErrorColector, object> op) : base(symbol, line, column)
        {
            _operator = op;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            return _operator(Left, Right,context,colector);
        }
    }
}
