using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class FuncCallAST : AbstractAST
    {
        List<IAST> args;
        public FuncCallAST(string Name, int line, int column, IAST[] args) : base(Name, line, column)
        {
            this.args = args.ToList();
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            var f = context.GetFuncion(Symbol);
            try
            {
                var result = f.DynamicInvoke(args);
                return result;
            }
            catch (Exception ex) {
                if (ex is IndexOutOfRangeException)
                {
                    colector.AddError(new Error("Fuera de Rango deberias Redimensionar el Canvas", Line, Column, ErrorType.Semantic));
                }
                else { colector.AddError(new Error(ex.Message, Line, Column, ErrorType.Semantic)); }
                return null;
            }
        }
    }
}
