using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class GotoSignal
    {
        public string label { get; }
        public GotoSignal(string label)
        {
            this.label = label;
        }
    }
    public class GoToAST : AbstractAST
    {
        IAST condition;
        IAST _label;
        public GoToAST(string symbol,IAST label, IAST condition,int line, int column) : base(symbol, line, column)
        {
            this.condition = condition;
            _label =label;
        }

        public override object Eval(IContext context, IErrorColector colector)
        {
            Draw.Information.asts.Add(this);
            var c = condition.Eval(context, colector);
            try
            {
                var b = Convert.ToBoolean(c);
                if (b)
                {
                    return new GotoSignal((string)_label.Eval(context, colector));
                }
            }
            catch
            {
                colector.AddError(new Error($"La condicion de la sentencia goto debe ser un booleano, se ha recibido {c.GetType().Name}", Line, Column, ErrorType.Semantic));
            }
            return null;
        }
    }
}
