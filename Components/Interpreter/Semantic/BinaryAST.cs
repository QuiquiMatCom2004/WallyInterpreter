using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class BinaryAST : AbstractAST
    {
        private Func<IAST, IAST, IContext, IErrorColector, object> _operator;
        IAST Left ;
        IAST Right ;

        public BinaryAST(string symbol, int line, int column, Func<IAST, IAST, IContext, IErrorColector, object> op, IAST left, IAST rigth) : base(symbol, line, column)
        {
            _operator = op;
            Left = left;
            Right = rigth;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            return _operator(Left, Right,context,colector);
        }
    }
}
