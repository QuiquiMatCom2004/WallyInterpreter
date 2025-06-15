using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class GotoSignal
    {
        public string label { get; }
        public GotoSignal(string label)
        {
            this.label = label;
        }
    }
    public class GoToAST : AbstractAST
    {
        IAST condition;
        string _label;
        public GoToAST(IAST label, IAST condition,int line, int column) : base(label.Symbol, line, column)
        {
            this.condition = condition;
            _label =label.Symbol;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            var c = condition.Eval(context, colector);
            if(c is bool b && b)
            {
                return new GotoSignal(_label);
            }
            return null;
        }
    }
}
