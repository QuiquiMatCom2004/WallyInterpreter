using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Lexical
{
    public class NumberGrammar
    {
        public IGranmar Number;
        public NumberGrammar() {
            var SignedNumber = new GranmarSymbol("SignedNumber", false, GranmarSymbolType.NonTerminal);
            var Numb = new GranmarSymbol("Number", false, GranmarSymbolType.NonTerminal);
            var before_tail = new GranmarSymbol("before_tail", false, GranmarSymbolType.NonTerminal);
            var after_tail = new GranmarSymbol("after_tail", false, GranmarSymbolType.NonTerminal);
            var dot_notacion = new GranmarSymbol("dot_notacion", false, GranmarSymbolType.NonTerminal);
            var d0 = new GranmarSymbol("0", false, GranmarSymbolType.Terminal);
            var d1 = new GranmarSymbol("1", false, GranmarSymbolType.Terminal);
            var d2 = new GranmarSymbol("2", false, GranmarSymbolType.Terminal);
            var d3 = new GranmarSymbol("3", false, GranmarSymbolType.Terminal);
            var d4 = new GranmarSymbol("4", false, GranmarSymbolType.Terminal);
            var d5 = new GranmarSymbol("5", false, GranmarSymbolType.Terminal);
            var d6 = new GranmarSymbol("6", false, GranmarSymbolType.Terminal);
            var d7 = new GranmarSymbol("7", false, GranmarSymbolType.Terminal);
            var d8 = new GranmarSymbol("8", false, GranmarSymbolType.Terminal);
            var d9 = new GranmarSymbol("9", false, GranmarSymbolType.Terminal);
            var dot = new GranmarSymbol(".", false, GranmarSymbolType.Terminal);
            var plus = new GranmarSymbol("+", false, GranmarSymbolType.Terminal);
            var minus = new GranmarSymbol("-", false, GranmarSymbolType.Terminal);
            var epsilon = new GranmarSymbol("e", true, GranmarSymbolType.Terminal);

            Number = new Granmar.Granmar(SignedNumber);

            Number.AddProduction(SignedNumber, new IGranmarSymbol[] {plus, Numb} );
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { minus, Numb });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d0, before_tail });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d1, before_tail });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d2, before_tail });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d3, before_tail });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d4, before_tail });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d5, before_tail });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d6, before_tail });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d7, before_tail });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d8, before_tail });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d9, before_tail });

            Number.AddProduction(Numb, new IGranmarSymbol[] { d0, before_tail });
            Number.AddProduction(Numb, new IGranmarSymbol[] { d1, before_tail });
            Number.AddProduction(Numb, new IGranmarSymbol[] { d2, before_tail });
            Number.AddProduction(Numb, new IGranmarSymbol[] { d3, before_tail });
            Number.AddProduction(Numb, new IGranmarSymbol[] { d4, before_tail });
            Number.AddProduction(Numb, new IGranmarSymbol[] { d5, before_tail });
            Number.AddProduction(Numb, new IGranmarSymbol[] { d6, before_tail });
            Number.AddProduction(Numb, new IGranmarSymbol[] { d7, before_tail });
            Number.AddProduction(Numb, new IGranmarSymbol[] { d8, before_tail });
            Number.AddProduction(Numb, new IGranmarSymbol[] { d9, before_tail });

            Number.AddProduction(before_tail, new IGranmarSymbol[] { d0, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { d1, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { d2, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { d3, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { d4, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { d5, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { d6, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { d7, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { d8, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { d9, before_tail });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { epsilon });
            Number.AddProduction(before_tail, new IGranmarSymbol[] { dot, after_tail });

            Number.AddProduction(Numb, new IGranmarSymbol[] { d0, dot_notacion });
            Number.AddProduction(SignedNumber, new IGranmarSymbol[] { d0, dot_notacion });
            Number.AddProduction(dot_notacion, new IGranmarSymbol[] { dot, after_tail });

            Number.AddProduction(after_tail, new IGranmarSymbol[] { d0, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { d1, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { d2, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { d3, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { d4, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { d5, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { d6, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { d7, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { d8, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { d9, after_tail });
            Number.AddProduction(after_tail, new IGranmarSymbol[] { epsilon });


        }
    }
}
