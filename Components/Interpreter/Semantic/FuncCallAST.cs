using System.Reflection;
using WallyInterpreter.Components.Draw;
using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class FuncCallAST : AbstractAST
    {
        List<IAST> args;
        IAST Name;
        public FuncCallAST(string symbol, int line, int column, IAST Name,IAST[] args) : base(symbol, line, column)
        {
            this.args = args.ToList();
            this.Name = Name;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            string name = (string)Name.Eval(context, colector);
            var f = context.GetFuncion(name);
            try
            {
                List<object> _params = new List<object>();
                foreach(var p in args)
                {
                    _params.Add(p.Eval(context,colector));
                }
                _params.Add(context.GetVariables("Canvas"));
                var result = f.DynamicInvoke(_params);
                return result;
            }
            catch (Exception ex) {
                if (ex is TargetInvocationException)
                    colector.AddError(new Error($"Ha habido un error al llamar el metodo {name}", Line, Column, ErrorType.Semantic));
                else if (ex is IndexOutOfRangeException)
                {
                    colector.AddError(new Error("Fuera de Rango deberias Redimensionar el Canvas", Line, Column, ErrorType.Semantic));
                }
                else { colector.AddError(new Error(ex.Message, Line, Column, ErrorType.Semantic)); }
                return null;
            }
        }
    }
}
