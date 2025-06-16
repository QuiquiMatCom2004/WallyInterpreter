using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class LabelAST : AbstractAST
    {
        public LabelAST(string symbol, int line, int column) : base(symbol, line, column)
        {
        }

        public override object Eval(IContext context, IErrorColector colector){Draw.Information.asts.Add(this); return null; }
        
    }
}
