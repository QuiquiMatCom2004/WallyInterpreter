using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class VariableAST : AbstractAST
    {
        public IAST _name;
        public VariableAST(string symbol, int line, int column, IAST variable) : base(symbol, line, column)
        {
            _name = variable;
        }
        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            try
            {
                if (context.GetVariables((string)_name.Eval(context,colector)) == null)
                {
                    throw new Exception("La variable no tiene ningun valor asignado");
                }
                return context.GetVariables((string)_name.Eval(context, colector));
            }
            catch (Exception e)
            {
                colector.AddError(new Error(e.Message, Line, Column, ErrorType.Semantic));
                return null;
            }
        }
    }
}
