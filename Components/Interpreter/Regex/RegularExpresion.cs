using WallyInterpreter.Components.Interpreter.Automaton;
using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Regex
{
    public class RegularExpresion : IRegularExpresion
    {
        public IAutomaton<char> Regex(IGranmar granmar)
        {
            /*
             Para convertir una gramatica en un automata tenemos que saber
             El automata Requiere de un conjunto de estados y un alfabeto
             Tal que cada estado conozca a donde transiciona
             La Gramatica conoce que Symbolos son iniciales, terminales,
             Finales y cuales son sus producciones
             */

            //1-Definir los estados

            List<IState<char>> states = new List<IState<char>>();
            foreach(var symbol in granmar.NonTerminals())
            {
                states.Add(new State<char>(symbol.Symbol(), false, false));
            }
            states.Add(new State<char>("Final", true, false));
            //2-Definir las transiciones
            foreach (var symbol in granmar.NonTerminals()) { 
                var productions = granmar.GetProduction(symbol);
                var actualState = states.Find(st => st.ID() == symbol.Symbol());
                foreach (var production in productions) {
                    if (production.Length > 2||production.Length < 1) throw new Exception("Not Regular Grammar");
                    switch(production.Length)
                    {
                        case 2:
                            if (production[0].Type() == production[1].Type()) {
                                throw new Exception("Not Regular Grammar");
                            }
                            switch(production[0].Type())
                            {
                                case GranmarSymbolType.Terminal:
                                    if (production[0].Epsilon())
                                        throw new Exception("this grammar is not regular Bad production " + symbol.Symbol());
                                    var transitionState = states.Find(s => s.ID() == production[1].Symbol());
                                    if (transitionState.HasTransition(production[0].Symbol()[0]))
                                    {
                                        var newState = states.Find(s => s.ID() == actualState.Next(production[0].Symbol()[0]).ID());
                                        newState.AddEpsilon(transitionState);
                                    }
                                    else
                                    {
                                        actualState.AddTransition(production[0].Symbol()[0],transitionState);
                                    }
                                    break;
                                case GranmarSymbolType.NonTerminal:
                                    if (production[1].Epsilon())
                                        throw new Exception("This grammar is not regular " + symbol.Symbol());
                                    var transitioState = states.Find(s => s.ID() == production[0].Symbol());
                                    actualState.AddEpsilon(transitioState);
                                    transitioState.AddTransition(production[1].Symbol()[0], states.Last());
                                    break;
                                default:
                                    throw new Exception("Type " + symbol.Type().ToString() +" is not defined");
                            }
                            break;
                        case 1:
                            switch(production[0].Type())
                            {
                                case GranmarSymbolType.Terminal:
                                    switch (production[0].Epsilon())
                                    {
                                        case true:
                                            actualState.AddEpsilon(states.Last());
                                            break;
                                        default:
                                            actualState.AddTransition(production[0].Symbol()[0], states.Last());
                                            break;
                                    }
                                 break;
                                default:
                                    throw new Exception("This granmar is not regular");
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            //3-definir el alfabeto
            List<char> alphabet = new List<char>();
            foreach(var t in granmar.Terminals())
            {
                alphabet.Add(t.Symbol()[0]);
            }
            return new Automaton<char>(states.Find(s => s.ID() == granmar.StartSymbol().Symbol()),states,alphabet.ToArray());
        }
    }
}
