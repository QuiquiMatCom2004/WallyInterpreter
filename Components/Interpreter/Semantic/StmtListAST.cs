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
            int pos = 0;
            List<object> result = new List<object>();
            while (pos < statements.Count())
            {
                var statement = statements[pos];

                result.Add(statement.Eval(context, colector));
            }
            return result;
        }
    }
}
