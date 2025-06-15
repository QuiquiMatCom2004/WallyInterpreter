using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Parser
{
    public interface IItemLR0
    {
        string ID { get; }
        IGranmarSymbol Head { get; }
        IGranmarSymbol[] LeftPoint { get; }
        IGranmarSymbol[] RightPoint{ get; }
    }
}
