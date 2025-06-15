namespace WallyInterpreter.Components.Interpreter.Semantic
{
    public interface IContext
    {
        /// <summary>
        /// Gets the value of variables.
        /// </summary>
        /// <param name="name">The name of variable</param>
        /// <returns>return null if the name don't exist in the scope</returns>
        object GetVariables(string name);
        void SetVariables(string name, object value);
        /// <summary>
        /// Gets the value of constants variables.
        /// </summary>
        /// <param name="name">The name of variable</param>
        /// <returns>return null if the name don't exist in the scope</returns>

        object GetInmutableValues(string name);
        void DefineInmutableValues(string name, object value);
        /// <summary>
        /// if exist return the function defined by the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Any function Predeterminate in lenguaje</returns>
        Delegate GetFuncion(string name);
        void DefineFunctions(string name,Delegate function);
        IContext Parent();

    }
}
