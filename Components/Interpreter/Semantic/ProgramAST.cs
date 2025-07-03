using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class ProgramAST : AbstractAST
    {
        IAST Block;
        public ProgramAST(string symbol, int line, int column, IAST block) : base(symbol, line, column)
        {
            Block = block;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            var block = (StmtListAST)Block;
            var localcontext = new Context(context);
            int pos = 0;
            Dictionary<string, int> _labelPos = new Dictionary<string, int>();
            for (int i = 0; i < block.statements.Count; i++)
                if (block.statements[i] is LabelAST lbl)
                    _labelPos[(string)lbl.Eval(context, colector)] = i;
            while (pos < block.statements.Count) {
                var stmt= block.statements[pos];
                try
                {
                    var res = stmt.Eval(context, colector);
                if (res is GotoSignal signal)
                {
                    if (!_labelPos.ContainsKey(signal.label))
                    {
                        colector.AddError(new Error($"el label {signal.label} no existe en el context", stmt.Line, stmt.Column, ErrorType.Semantic));
                        continue;
                    }
                    pos = _labelPos[signal.label];
                }
                else
                    pos++;
                }catch(Exception e)
                {
                    colector.AddError(new Error(e.Message, stmt.Line, stmt.Column, ErrorType.Semantic));
                }
            }
            return null;
        }
    }
}
