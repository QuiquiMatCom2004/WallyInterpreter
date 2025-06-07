using WallyInterpreter.Components.Interpreter.Automaton;
using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Regex
{
    public interface IRegularExpresion
    {
        IAutomaton<char> Regex(IGranmar granmar);
    }
}
