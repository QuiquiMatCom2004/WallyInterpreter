
namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public class Context(IContext? parent = null) : IContext
    {
        Dictionary<string, object> _variables = new Dictionary<string, object>();
        Dictionary<string,Delegate> _functions = new Dictionary<string,Delegate>();
        IContext? _parent = parent;
        public void DefineFunctions(string name, Delegate function)
        {
            _functions.Add(name, function);
            if (_parent != null && _parent.GetFuncion(name) != null) { 
                _parent.DefineFunctions(name, function);
            }
        }

        public Delegate GetFuncion(string name)
        {
            if (!_functions.ContainsKey(name)) {
                if (_parent != null)
                    return _parent.GetFuncion(name);
                return null;
            }
            return _functions[name];
        }
        public object GetVariables(string name)
        {
            if (!_variables.ContainsKey(name)) { 
                if(_parent != null)
                    return _parent.GetVariables(name);
                return null;
            }
            return _variables[name];
        }

        public IContext? Parent()
        {
            return _parent;
        }

        public void SetVariables(string name, object value)
        {
            _variables[name] = value;
            if (_parent != null && _parent.GetVariables(name) != null) { 
                _parent.SetVariables(name, value); 
            }
        }
    }
}
