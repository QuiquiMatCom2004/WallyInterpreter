using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Lexical
{
    public class SymbolsGranmar
    {
        public IGranmar Symbols;
        public SymbolsGranmar() {
            var symbols = new string[]
            {
                "(",")","[","]","<-",","
            };
            Symbols = GranmarTools.GetWordsGrammar(symbols);
        }
    }
}
