using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Lexical
{
    public class StringGrammar
    {
        public IGranmar StringGranmar;
        public StringGrammar() {
            var startSymbol = new GranmarSymbol("start_symbol",false,GranmarSymbolType.NonTerminal);
            var tail = new GranmarSymbol("tail",false, GranmarSymbolType.NonTerminal);
            var double_quotes = new GranmarSymbol("\"",false,GranmarSymbolType.Terminal);
            List<GranmarSymbol> symbols = new List<GranmarSymbol>();
            for(int i = 32; i <= 126; i++)
            {
                symbols.Add(new GranmarSymbol(((char)i).ToString(), false,GranmarSymbolType.Terminal));
            }
            StringGranmar = new Granmar.Granmar(startSymbol);
            StringGranmar.AddProduction(startSymbol, new IGranmarSymbol[] { double_quotes, tail });
            foreach(var symbol in symbols)
            {
                StringGranmar.AddProduction(tail, new IGranmarSymbol[] { symbol, tail });
            }
            StringGranmar.AddProduction(tail, new IGranmarSymbol[] { double_quotes });
        }
    }
}
