using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class VariableAST : AbstractAST
    {
        public string _name;
        public VariableAST(string symbol,int line,int column,string variable):base(symbol,line,column) { 
         _name = variable;
        }
        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            try
            {
                if (context.GetVariables(_name) == null)
                {
                    throw new Exception("La variable no tiene ningun valor asignado");
                }
                return context.GetVariables(_name);
            }
            catch(Exception e) {
                colector.AddError(new Error(e.Message,Line,Column,ErrorType.Semantic));
                return null;
            }
        }
    }
}
