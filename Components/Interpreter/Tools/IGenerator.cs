using Microsoft.AspNetCore.Components.Web;

namespace WallyInterpreter.Components.Interpreter.Tools
{
    public interface IGenerator<T>
    {
        bool Next();
        T Current();
    }
}
