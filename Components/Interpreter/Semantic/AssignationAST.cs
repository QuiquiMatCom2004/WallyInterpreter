using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class AssignationAST : AbstractAST
    {
        private IAST expresion;
        public AssignationAST(string variable, int line, int column, IAST expresion) : base(variable, line, column)
        {
            this.expresion = expresion;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            var val = expresion.Eval(context, colector);
            //Cambiar en el contexto global
            context.SetVariables(Symbol, val);
            return val;
        }
    }
}
