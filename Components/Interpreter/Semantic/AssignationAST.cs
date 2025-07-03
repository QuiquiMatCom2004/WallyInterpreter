using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class AssignationAST : AbstractAST
    {
        private IAST variable;
        private IAST expresion;
        public AssignationAST(string symbol, int line, int column, IAST variable,IAST expresion) : base(symbol, line, column)
        {
            this.expresion = expresion;
            this.variable = variable;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            var val = expresion.Eval(context, colector);
            //Cambiar en el contexto global
            context.SetVariables((string)variable.Eval(context,colector), val);
            return val;
        }
    }
}
