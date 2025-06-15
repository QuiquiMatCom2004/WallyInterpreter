using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Lexical
{
    public class IdentifierGrammar
    {
        public IGranmar Identifier;
        public IdentifierGrammar() {
            var startSymbol = new GranmarSymbol("start_symbol", false, GranmarSymbolType.NonTerminal);
            var tail = new GranmarSymbol("tail",false, GranmarSymbolType.NonTerminal);
            var epsilon = new GranmarSymbol("e",true, GranmarSymbolType.Terminal);
            var down_line = new GranmarSymbol("_", false, GranmarSymbolType.Terminal);
            var digits = new List<GranmarSymbol>();
            var letters = new  List<GranmarSymbol>();
            Identifier = new Granmar.Granmar(startSymbol);

            for(int i = 48; i <= 57;i++)
            {
                digits.Add(new GranmarSymbol(((char)i).ToString(), false, GranmarSymbolType.Terminal));
            }
            for(int i = 65;i<= 90;i++)
            {
                letters.Add(new GranmarSymbol(((char)i).ToString(), false, GranmarSymbolType.Terminal));
            }
            for(int i = 97; i <=122 ;i++)
            {
                letters.Add(new GranmarSymbol(((char)i).ToString(), false, GranmarSymbolType.Terminal));
            }
            foreach (var letter in letters)
            {
                Identifier.AddProduction(startSymbol, new IGranmarSymbol[] { letter, tail });
                Identifier.AddProduction(tail, new IGranmarSymbol[] { letter, tail });
            }
            foreach (var digit in digits)
                Identifier.AddProduction(tail, new IGranmarSymbol[] { digit, tail });
            Identifier.AddProduction(tail, new IGranmarSymbol[] { down_line, tail });
            Identifier.AddProduction(tail, new IGranmarSymbol[] { epsilon });
        }
    }
}
