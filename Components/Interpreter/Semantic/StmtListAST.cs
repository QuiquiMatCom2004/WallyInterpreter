using System.Collections.Generic;
using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class StmtListAST : AbstractAST
    {
        public List<IAST> statements { get; }
        
        public StmtListAST(string symbol,IAST[] statements, int line, int column) : base(symbol, line, column)
        {
            this.statements = statements.ToList();
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            var localcontext = new Context(context);
            int pos = 0;
            Dictionary<string, int> _labelPos = new Dictionary<string, int>();
            for (int i = 0; i < statements.Count; i++)
                if (statements[i] is LabelAST lbl)
                    _labelPos[(string)lbl.Eval(context,colector)] = i;
            while (pos < statements.Count())
            {
                var statement = statements[pos];
                var res = statement.Eval(localcontext, colector);
                if (res is GotoSignal sig)
                {
                    if (!_labelPos.ContainsKey(sig.label))
                    {
                        colector.AddError(new Error($"Label {sig.label} no declarado", statement.Line, statement.Column, ErrorType.Semantic));
                        return null;
                    }
                    pos = _labelPos[sig.label];
                }
                else
                {
                    pos++;
                }
            }
            return statements;
        }
    }
}
