using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Parser
{
    public class ItemLR0(IGranmarSymbol head, IGranmarSymbol[] left, IGranmarSymbol[] right) : IItemLR0
    {
        public string ID => GetID();

        public IGranmarSymbol Head => head;

        public IGranmarSymbol[] LeftPoint => left;

        public IGranmarSymbol[] RightPoint =>right;
        private string GetID()
        {
            string result = Head.Symbol() + "-->";

            foreach (var item in LeftPoint) {
                result += " " + item.Symbol();
            }
            result += ".";
            foreach (var item in RightPoint)
            {
                result += " " + item.Symbol();
            }
            return result;
        }
    }
}
