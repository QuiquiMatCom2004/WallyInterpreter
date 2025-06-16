using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class StmtListAST : AbstractAST
    {
        public List<IAST> statements { get; }
        private Dictionary<string, int> _labelPos;
        public StmtListAST(IAST[] statements, int line, int column) : base("BLOCK", line, column)
        {
            this.statements = statements.ToList();
            _labelPos = BuildLabelMap(statements.ToList());
        }
        private Dictionary<string, int> BuildLabelMap(List<IAST> list)
        {
            var map = new Dictionary<string, int>();
            for (int i = 0; i < list.Count; i++)
                if (list[i] is LabelAST lbl)
                    map[lbl.Symbol] = i;
            return map;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            var localcontext = new Context(context);
            int pos = 0;
            while (pos < statements.Count()) { 
                var statement = statements[pos];
                var res = statement.Eval(localcontext, colector);
                if(res is GotoSignal sig)
                {
                    if (!_labelPos.ContainsKey(sig.label))
                    {
                        colector.AddError(new Error($"Label {sig.label} no declarado",statement.Line,statement.Column,ErrorType.Semantic));
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
