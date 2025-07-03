using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.Parser
{
    public interface IParser
    {
        void Parse(IToken token, IErrorColector collector);
        IAST GetAST();
        void SetReduction(string reduction_id, Func<IAST[],string,IAST> reductor);
        string EndMarker();
    }
    public interface IParserSLR:IParser
    {
        string StartState();
        Dictionary<int, Dictionary<string, ActionStruct>> ActionTable();
        Dictionary<int, Dictionary<string, ReduceStruct>> ReduceTable();

        void Reset();
    }
}
