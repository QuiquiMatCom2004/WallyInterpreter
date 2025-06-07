using Microsoft.AspNetCore.Components.Forms;
using WallyInterpreter.Components.Interpreter.Automaton;
using WallyInterpreter.Components.Interpreter.Granmar;
using WallyInterpreter.Components.Interpreter.Lexer;

namespace WallyInterpreter.Components.Services
{
    public class Backend
    {
        private IGranmar ChargeNumberGranmar()
        {
           

            var n_0 = new GranmarSymbol("0",false, GranmarSymbolType.Terminal);
            var n_1 = new GranmarSymbol("1",false, GranmarSymbolType.Terminal);
            var n_2 = new GranmarSymbol("2",false, GranmarSymbolType.Terminal);
            var n_3 = new GranmarSymbol("3",false, GranmarSymbolType.Terminal);
            var n_4 = new GranmarSymbol("4",false, GranmarSymbolType.Terminal);
            var n_5 = new GranmarSymbol("5",false, GranmarSymbolType.Terminal);
            var n_6 = new GranmarSymbol("6",false, GranmarSymbolType.Terminal);
            var n_7 = new GranmarSymbol("7",false, GranmarSymbolType.Terminal);
            var n_8 = new GranmarSymbol("8",false, GranmarSymbolType.Terminal);
            var n_9 = new GranmarSymbol("9",false, GranmarSymbolType.Terminal);
           
            
            var Number = new GranmarSymbol("N", false, GranmarSymbolType.NonTerminal);
            
            var epsilon = new GranmarSymbol("e", true, GranmarSymbolType.Terminal);
            IGranmar granmar = new Granmar(Number);

            granmar.AddProduction(Number, new IGranmarSymbol[] { n_0 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { n_1 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { n_2 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { n_3 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { n_4 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { n_5 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { n_6 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { n_7 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { n_8 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { n_9 , Number});
            granmar.AddProduction(Number, new IGranmarSymbol[] { epsilon });

            return granmar;
        }
        private IGranmar ChargeOperatorGranmar()
        {
            
            var minus = new GranmarSymbol("-", false, GranmarSymbolType.Terminal);
            var mult = new GranmarSymbol("*", false, GranmarSymbolType.Terminal);
            var div = new GranmarSymbol("/", false, GranmarSymbolType.Terminal);
            var epsilon = new GranmarSymbol("e", true, GranmarSymbolType.Terminal);
            var Operator = new GranmarSymbol("O", false, GranmarSymbolType.NonTerminal);
            var sum = new GranmarSymbol("+", false, GranmarSymbolType.Terminal);
            var granmar = new Granmar(Operator);
            granmar.AddProduction(Operator, new IGranmarSymbol[] { minus, Operator });
            granmar.AddProduction(Operator, new IGranmarSymbol[] { sum, Operator });
            granmar.AddProduction(Operator, new IGranmarSymbol[] { mult, Operator });
            granmar.AddProduction(Operator, new IGranmarSymbol[] { div, Operator });
            granmar.AddProduction(Operator, new IGranmarSymbol[] { epsilon });
            return granmar;
        }
        private IGranmar ChargeSymbolGranmar()
        {
            var lparent = new GranmarSymbol("(", false, GranmarSymbolType.Terminal);
            var rparent = new GranmarSymbol(")", false, GranmarSymbolType.Terminal);
            var Parent = new GranmarSymbol("P", false, GranmarSymbolType.NonTerminal);
            var granmar = new Granmar(Parent);
            granmar.AddProduction(Parent, new IGranmarSymbol[] { lparent });
            granmar.AddProduction(Parent, new IGranmarSymbol[] { rparent });
            return granmar;

        }
        public string TestLexer(string code)
        {
            var lex = new Lexer();
            lex.LoadCode(code);
            lex.AddTokenExpresion(Interpreter.Tokens.Tokentype.Number, 1, ChargeNumberGranmar());
            lex.AddTokenExpresion(Interpreter.Tokens.Tokentype.Operator,2,ChargeOperatorGranmar());
            lex.AddTokenExpresion(Interpreter.Tokens.Tokentype.Symbol, 3, ChargeSymbolGranmar());
            string result = "";
            int i = 0;
            bool b = true;
            do
            {
                b = lex.Next();
                i++;
                result += $"Token {i}: (" + lex.Current().ToString() + " ) ";
            } while (b);
                return result;
        }
        public IAutomaton<T> CreateAutomaton<T>(IState<T> start, List<IState<T>> states,T[] alphabeth) {
            return new Automaton<T>(start, states, alphabeth);
        }
        public IAutomaton<T> CreateAutomatonBySequence<T>(T[] sequence,string id)
        {
            return AutomatonOperatorsAndTools<T>.SequenceAutomaton(sequence, id);
        }
        public IAutomaton<T> UnionAutomaton<T>(IAutomaton<T> aut1,IAutomaton<T> aut2)
        {
            return AutomatonOperatorsAndTools<T>.Union(aut1 , aut2);
        }
        public IAutomaton<T> ConcatAutomaton<T>(IAutomaton<T> aut1, IAutomaton<T> aut2)
        {
            return AutomatonOperatorsAndTools<T>.Concat(aut1, aut2);
        }
        public IAutomaton<T> CopyAutomaton<T>(IAutomaton<T> aut1)
        {
            return AutomatonOperatorsAndTools<T>.CopyAutomaton(aut1);
        }
        public string GetAutomaton<T>(IAutomaton<T> aut)
        {
            return aut.ToString();
        }
        public bool Match<T>(T[] s, IAutomaton<T> aut)
        {
            return AutomatonOperatorsAndTools<T>.Match(s, aut);
        }
        public string GetGranmar(IGranmar granmar)
        {
            return granmar.ToString();
        }
        public IGranmar CreateGranmar(IGranmarSymbol start)
        {
            return new Granmar(start);
        }
        public IGranmar AddWord(IGranmar granmar , string word)
        {
            return GranmarTools.AddWordToGranmar(granmar , word);
        }
        public IGranmar GetWordsGranmar(string[] words)
        {
            return GranmarTools.GetWordsGrammar(words);
        }
        public IGranmar UnionGranmar(IGranmar[] granmars, string start)
        {
            return GranmarTools.Union(granmars, start);
        }
    }
}
