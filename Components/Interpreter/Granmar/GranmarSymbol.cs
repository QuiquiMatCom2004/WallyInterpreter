namespace WallyInterpreter.Components.Interpreter.Granmar
{
    public class GranmarSymbol : IGranmarSymbol
    {
        private string _symbol;
        private bool _epsilon;
        private GranmarSymbolType _type;
        public GranmarSymbol(string symbol,bool epsilon , GranmarSymbolType type)
        {
            _symbol = symbol;
            _epsilon = epsilon;
            _type = type;
        }
        public bool Epsilon()
        {
            return _epsilon;
        }

        public string Symbol()
        {
            return _symbol;
        }

        public GranmarSymbolType Type()
        {
            return _type;    
        }
    }
}
