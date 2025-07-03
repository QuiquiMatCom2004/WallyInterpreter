using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Sintactic
{
    public class DummyGranmar
    {
        public IGranmar Dummy;
        public DummyGranmar()
        {
            var E = new GranmarSymbol("E", false, GranmarSymbolType.NonTerminal);
            var T = new GranmarSymbol("T", false, GranmarSymbolType.NonTerminal);
            var F = new GranmarSymbol("F", false, GranmarSymbolType.NonTerminal);

            var plus = new GranmarSymbol("+", false, GranmarSymbolType.Terminal);
            var mult = new GranmarSymbol("*", false, GranmarSymbolType.Terminal);
            var LPar = new GranmarSymbol("(", false, GranmarSymbolType.Terminal);
            var RPar = new GranmarSymbol(")", false, GranmarSymbolType.Terminal);
            var id = new GranmarSymbol("id", false, GranmarSymbolType.Terminal);
            var granmar = new Granmar.Granmar(E);
            granmar.AddProduction(E, new[] { E, plus, T });
            granmar.AddProduction(E, new[] { T });
            granmar.AddProduction(T, new[] { T, mult, F });
            granmar.AddProduction(T, new[] { F });
            granmar.AddProduction(F, new[] { LPar, E, RPar });
            granmar.AddProduction(F, new[] { id });
            Dummy = granmar;
        }
    }
}
