
namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class Context(IContext? parent = null) : IContext
    {
        Dictionary<string, object> _inmutable = new Dictionary<string, object>();
        Dictionary<string, object> _variables = new Dictionary<string, object>();
        Dictionary<string,Delegate> _functions = new Dictionary<string,Delegate>();
        IContext? _parent = parent;
        public void DefineFunctions(string name, Delegate function)
        {
            _functions.Add(name, function);
        }


        public void DefineInmutableValues(string name, object value)
        {
            _inmutable.Add(name, value);
        }

        public Delegate GetFuncion(string name)
        {
            return _functions[name];
        }

        public object GetInmutableValues(string name)
        {
            return _inmutable[name];
        }

        public object GetVariables(string name)
        {
            return _variables[name];
        }

        public IContext? Parent()
        {
            return _parent;
        }

        public void SetVariables(string name, object value)
        {
            _variables[name] = value;
        }
    }
}
