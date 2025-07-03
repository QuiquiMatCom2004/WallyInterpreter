using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class LabelAST : AbstractAST
    {
        private IAST _label;
        public LabelAST(string symbol, int line, int column, IAST label) : base(symbol, line, column)
        {
            _label = label;
        }

        public override object Eval(IContext context, IErrorColector colector) { Draw.Information.asts.Add(this); return _label.Eval(context,colector); }

    }
}
