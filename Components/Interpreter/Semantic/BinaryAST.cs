using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class BinaryAST : AbstractAST
    {
        private Func<IAST, IAST, IContext, IErrorColector, object> _operator;
        public IAST Left ;
        public IAST Right ;

        public BinaryAST(string symbol, int line, int column, Func<IAST, IAST, IContext, IErrorColector, object> op) : base(symbol, line, column)
        {
            _operator = op;
            Left = null;
            Right = null;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            return _operator(Left, Right,context,colector);
        }
    }
}
