using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Granmar;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.Parser
{
    public class ParserSLR0 : IParserSLR
    {
        private IGranmarSymbol _endmarker;
        private Dictionary<int,Dictionary<string,ParserAction>> _action = new Dictionary<int, Dictionary<string, ParserAction>>();
        private Dictionary<int,Dictionary<string,int>> _goTable = new Dictionary<int, Dictionary<string, int>>();
        private Dictionary<int, Dictionary<string, ReduceStruct>> _reduce;
        private Dictionary<string, Func<IAST[], string, IAST>> _reduction;
        private int _initialState;
        private int _currentState;
        private IItemCollectionLR0[] _states;
        private Func<IToken, string, IAST> _astEngine;
        private Stack<IAST> _stack;
        private Stack<int> _stackStates;
        public ParserSLR0(IGranmar g, IGranmarSymbol endmarker, Func<IToken,string,IAST> astengine) 
        { 
            _reduce = new Dictionary<int, Dictionary<string, ReduceStruct>>();
            _stack = new Stack<IAST>();
            _endmarker = endmarker;
            _astEngine = astengine;
            _reduction = new Dictionary<string, Func<IAST[], string, IAST>>();
            var granmar = GranmarTools.Augment(g);
            granmar.MakeFirstAndFollow(endmarker);
            ConstructActionTable(granmar);
            _stackStates = new Stack<int>();
            _stackStates.Push(_initialState);
        }
        private void ConstructActionTable(IGranmar gAugmented)
        {
            var tools = new ParserTools();
            var action = new Dictionary<int, Dictionary<string, ParserAction>>();
            var goTable = new Dictionary<int, Dictionary<string, int>>();
            var C = tools.GetCanonicalLR0Collection(gAugmented);
            for (int i = 0; i < C.Length; i++) {
                foreach (var itemLR0 in C[i])
                {
                    if(itemLR0.RightPoint.Length > 0)
                    {
                        foreach (var terminal in gAugmented.Terminals())
                        {
                            var go = tools.GOTO(C[i], terminal, gAugmented);
                            if(go != null)
                            {
                                if (!action.ContainsKey(i))
                                    action[i] = new Dictionary<string, ParserAction>();
                                action[i][terminal.Symbol()] = ParserAction.Shift;
                                if (!goTable.ContainsKey(i))
                                    goTable[i] = new Dictionary<string, int>();
                                goTable[i][terminal.Symbol()] = C.ToList().FindIndex(coll => coll.ID == go.ID);
                            }
                        }
                    }
                    else if(itemLR0.Head.Symbol() == gAugmented.StartSymbol().Symbol())
                    {
                        if (!action.ContainsKey(i))
                        {
                            action[i] = new Dictionary<string, ParserAction>();
                        }
                        action[i][_endmarker.Symbol()] = ParserAction.Accept;
                    }
                    else if(itemLR0.RightPoint.Length == 0)
                    {
                        foreach(var terminal in  gAugmented.Follow(itemLR0.Head))
                        {
                            if (!action.ContainsKey(i))
                            {
                                action[i] = new Dictionary<string, ParserAction>();
                            }
                            action[i][terminal.Symbol()] = ParserAction.Reduce;
                            if (!_reduce.ContainsKey(i))
                                _reduce[i] = new Dictionary<string, ReduceStruct>();
                            _reduce[i][terminal.Symbol()] = new ReduceStruct(itemLR0.Head.Symbol(),itemLR0.LeftPoint.Select(s => s.Symbol()).ToArray());
                        }
                    }
                }
                foreach (var nonTerminal in gAugmented.NonTerminals())
                {
                    var go = tools.GOTO(C[i], nonTerminal, gAugmented);
                    if (go != null)
                    {
                        if (!goTable.ContainsKey(i))
                            goTable[i] = new Dictionary<string, int>();
                        goTable[i][nonTerminal.Symbol()] = C.ToList().FindIndex(coll => coll.ID == go.ID);
                    }
                }
                foreach(var itemLR0 in C[i])
                {
                    if (itemLR0.Head.Symbol() == gAugmented.StartSymbol().Symbol() && itemLR0.LeftPoint.Length == 0)
                    {
                        _initialState = i;
                    }
                }
            }

            

            _action = action;
            _goTable = goTable;
            _states = C;
        }
        public Dictionary<int, Dictionary<string, ActionStruct>> ActionTable()
        {
            var result = new Dictionary<int, Dictionary<string, ActionStruct>>();
            foreach(var k in _action.Keys)
            {
                foreach(var s in _action[k].Keys)
                {
                    result[k].Add(s, new ActionStruct(_action[k][s], _goTable[k][s].ToString()));
                }
            }
            return result;
        }

        public string EndMarker()
        {
            return _endmarker.Symbol();
        }

        public IAST GetAST()
        {
            return _stack.Peek(); 
        }

        public void Parse(IToken token, IErrorColector collector)
        {
            var ast = _astEngine(token, _endmarker.Symbol());
            bool reduced = true;
            while(reduced)
            {
                if (_action[_currentState].ContainsKey(ast.Symbol) && _action[_currentState][ast.Symbol] == ParserAction.Shift)
                {
                    _stack.Push(ast);
                    _currentState = _goTable[_currentState][ast.Symbol];
                    _stackStates.Push(_currentState);
                    reduced = false;
                }
                else if (_action[_currentState].ContainsKey(ast.Symbol) && _action[_currentState][ast.Symbol] == ParserAction.Accept)
                {
                    return;
                }
                else if (_action[_currentState].ContainsKey(ast.Symbol) && _action[_currentState][ast.Symbol] == ParserAction.Reduce)
                {
                    reduced = true;
                    List<IAST> asts = new List<IAST>();
                    for (int i = 0; i < _reduce[_currentState][ast.Symbol].Symbols.Count; i++)
                    {
                        asts.Add(_stack.Pop());
                        _stackStates.Pop();
                    }
                    asts.Reverse();
                    var newAST = _reduction[_reduce[_currentState][ast.Symbol].NewSymbol + "-->" + string.Join(" ", _reduce[_currentState][ast.Symbol].Symbols)](asts.ToArray(), _reduce[_currentState][ast.Symbol].NewSymbol);
                    _stack.Push(newAST);
                    _currentState = _stackStates.Peek();
                    _currentState = _goTable[_currentState][newAST.Symbol];
                    _stackStates.Push(_currentState);

                }
                else
                    throw new Exception($"El estado actual I{_currentState} no contiene el simbolo {ast.Symbol}");
            }
        }

        public Dictionary<int, Dictionary<string, ReduceStruct>> ReduceTable()
        {
            return _reduce;
        }

        public void Reset()
        {
            _stack.Clear();
            _stackStates.Clear();
            _stackStates.Push(_initialState);
            _currentState = _initialState;
        }

        public void SetReduction(string reduction_id, Func<IAST[], string, IAST> reductor)
        {
            _reduction[reduction_id] = reductor;
        }

        public string StartState()
        {
            return _states[_initialState].ID;
        }

    }
}
