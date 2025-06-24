using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic;
using WallyInterpreter.Components.Interpreter.Automaton;
using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Granmar;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.Parser
{
    public class ParserSLR : IParserSLR
    {
        private List<IState<string>> _states = new List<IState<string>>();
        private IState<string> _startState;
        private List<IState<string>> _stateStack = new List<IState<string>>();
        private Dictionary<string, Dictionary<string, ActionStruct>> _action = new Dictionary<string, Dictionary<string, ActionStruct>>(StringComparer.Ordinal);
        private Dictionary<string, Dictionary<string, ReduceStruct>> _reduce = new Dictionary<string, Dictionary<string, ReduceStruct>>(StringComparer.Ordinal);
        private List<IAST> _stack = new List<IAST>();
        private Func<IToken, string, IAST> _ast_engine;
        private Dictionary<string, Func<IAST[],string,IAST>> _reductionFunction = new Dictionary<string, Func<IAST[], string, IAST>>();
        private string _endmarker;
        private List<string> _terminals = new List<string>();
        public ParserSLR(IGranmar g, IGranmarSymbol endmarker, Func<IToken, string, IAST> ast_engine)
        {
            _ast_engine = ast_engine;
            _endmarker = endmarker.Symbol();
            //Gramatica Augmented
            var G = GranmarTools.Augment(g);
            G.MakeFirstAndFollow(endmarker);

            
            //Construir el canonico para la gramatica aumentada
            var tools = new ParserTools();
            var canonical = tools.GetCanonicalLR0Collection(G);
            _states = canonical.Select((c, i) => {
                var s = new State<string>($"I{i}", true, false);
                return (IState<string>)s; 
            }).ToList();
            //Construccion del estado i construido a partir del canonico Ii 

            for (int i = 0; i < canonical.Length; i++) {
                foreach (var item in canonical[i]) {
                    if (item.RightPoint.Length > 0)
                    {
                        
                        foreach (var term in G.Terminals())
                        {
                            var go = tools.GOTO(canonical[i], term, G);
                            if (go != null)
                            {
                                var nextState = canonical.ToList().FindIndex(coll => coll.ID == go.ID);
                                _states[i].AddTransition(term.Symbol(), _states[nextState]);
                                if (!_action.ContainsKey(_states[i].ID()))
                                {
                                    _action[_states[i].ID()] = new Dictionary<string, ActionStruct>();
                                }
                                if (_action[_states[i].ID()].TryGetValue(term.Symbol(), out var act))
                                {
                                    if (act.Action != _action[_states[i].ID()][term.Symbol()].Action)
                                        throw new Exception("This grammar is not lr0");
                                }
                                _action[_states[i].ID()][term.Symbol()] = new ActionStruct(ParserAction.Shift, _states[nextState].ID());
                            }
                        }
                    }
                    else if (item.RightPoint.Length == 0 && item.Head.Symbol() != G.StartSymbol().Symbol())
                    {
                        foreach (var term in G.Follow(item.Head))
                        {
                            if (!_action.ContainsKey(_states[i].ID()))
                            {
                                _action[_states[i].ID()] = new Dictionary<string, ActionStruct>();
                            }
                            if (_action[_states[i].ID()].TryGetValue(term.Symbol(), out var act))
                            {
                                if (act.Action != _action[_states[i].ID()][term.Symbol()].Action)
                                    throw new Exception("This grammar is not lr0");
                            }
                            _action[_states[i].ID()][term.Symbol()] = new ActionStruct(ParserAction.Reduce, "");
                            if (!_reduce.ContainsKey(_states[i].ID()))
                            {
                                _reduce[_states[i].ID()] = new Dictionary<string, ReduceStruct>();
                            }
                            _reduce[_states[i].ID()][term.Symbol()] = new ReduceStruct(item.Head.Symbol(), item.LeftPoint.Select(s => s.Symbol()).ToArray());
                        }
                    }
                    else if (item.RightPoint.Length == 0 && item.Head.Symbol() == G.StartSymbol().Symbol())
                    {
                        if (!_action.ContainsKey(_states[i].ID()))
                            _action[_states[i].ID()] = new Dictionary<string, ActionStruct>();
                        _action[_states[i].ID()][_endmarker] = new ActionStruct(ParserAction.Accept, "");
                    }
                }

                foreach(var nt in G.NonTerminals())
                {
                    var go = tools.GOTO(canonical[i], nt, g);
                    if (go != null)
                    {
                        var nextState = canonical.ToList().FindIndex(coll => coll.ID == go.ID);
                        _states[i].AddTransition(nt.Symbol(),_states[nextState]);
                        if (!_action.ContainsKey(_states[i].ID()))
                            _action[_states[i].ID()] = new Dictionary<string, ActionStruct>();
                        if (_action[_states[i].ID()].TryGetValue(nt.Symbol(),out var act))
                        {
                            if (_action[_states[i].ID()][nt.Symbol()].Action != act.Action)
                                throw new Exception("this grammar its not lr0");
                        }
                        _action[_states[i].ID()][nt.Symbol()] = new ActionStruct(ParserAction.Shift, _states[nextState].ID());
                    }
                }
                //Faltan las ultimas reglas y algun retoque
            }
            _terminals = G.Terminals().Select(t => t.Symbol()).ToList();
            var indexStart = canonical.Select((c) =>
            {
                foreach (var item in c)
                {
                    if (item.LeftPoint.Length > 0)
                        return false;
                }
                return true;
            });
            _startState = _states[indexStart.ToList().FindIndex(b => b)];

            _stateStack.Add(_startState);

            string parsing_table = "";
            foreach(var state in _action.Keys)
            {
                foreach(var terminal in _action[state].Keys)
                {
                    parsing_table += $"{state},{terminal} ------> {_action[state][terminal].Action},{_action[state][terminal].NextState}\n";
                }
            }
            File.WriteAllText("ParserTableSave.txt", parsing_table);
        }
        public Dictionary<string, Dictionary<string, ActionStruct>> ActionTable()
        {
            return _action;
        }

        public string EndMarker()
        {
            return _endmarker;
        }

        public IAST GetAST()
        {
            return _stack[0];
        }

        public void Parse(IToken token, IErrorColector collector)
        {
            
            var ast = _ast_engine(token, _endmarker);
            var currentState = _stateStack.Last().ID();
            Console.WriteLine($"Incoming AST.Symbol = '{ast.Symbol}'");
            Console.WriteLine($"Terminals in action[{currentState}] = {string.Join(",", _action[currentState].Keys.Select(k => $"'{k}'"))}");
            Console.WriteLine($"Action {currentState} : {ast.Symbol}  -----> {_action[currentState][ast.Symbol].Action}");

            if (!_action[currentState].TryGetValue(ast.Symbol, out var action))
            {
                string msg = "expected ";
                foreach (var k in _action[currentState].Keys)
                {
                    if (_terminals.Contains(k))
                        msg += k + ", ";
                }
                if (ast.Symbol == _endmarker)
                {
                    collector.AddError(new Error($"Unexpected EOF symbol {msg}", ast.Line, ast.Column, ErrorType.Gramatical));
                }
                else
                {
                    collector.AddError(new Error($"Unexpected Symbol {ast.Symbol},{msg}", ast.Line, ast.Column, ErrorType.Gramatical));
                }
                return;
            }
            bool reduced = true;
            do
            {
                switch (action.Action)
                {
                    case ParserAction.Accept:
                        Draw.Information.actions.Enqueue(action.Action);
                        var next1 = _states.Find(s => s.ID() == action.NextState);
                        _stateStack.Add(next1);
                        _stack.Add(ast);
                        reduced = false;
                        break;
                    case ParserAction.Shift:
                        Draw.Information.actions.Enqueue(action.Action);
                        var next = _states.Find(s => s.ID() == action.NextState);
                        _stateStack.Add(next);
                        _stack.Add(ast);
                        reduced = false;
                        break;
                    case ParserAction.Reduce:
                        reduced = true;
                        Draw.Information.actions.Enqueue(action.Action);
                        var red = _reduce[currentState][ast.Symbol];
                        var head = red.NewSymbol;       
                        var rhs = red.Symbols;

                        var children = _stack.GetRange(_stack.Count - rhs.Count, rhs.Count);
                        _stack.RemoveRange(_stack.Count - rhs.Count, rhs.Count);
                        _stateStack.RemoveRange(_stateStack.Count - rhs.Count, rhs.Count);

                        var newAST = _reductionFunction[head + "-->" + string.Join(" ", rhs)](children.ToArray(), head);

                        _stack.Add(newAST);
                        var afterReduce = _stateStack.Last().ID();
                        action = _action[afterReduce][head];

                        _stateStack.Add(_stateStack.Find(s=> s.ID() == action.NextState));
                        break;
                }
            } while (reduced);
        }

        public Dictionary<string, Dictionary<string, ReduceStruct>> ReduceTable()
        {
            return _reduce;
        }

        public void SetReduction(string reduction_id, Func<IAST[], string, IAST> reductor)
        {
            _reductionFunction[reduction_id] = reductor;   
        }

        public string StartState()
        {
            return _startState.ID();
        }
        public void Reset()
        {
            _stateStack.Clear();
            _stateStack.Add(_startState);
            _stack.Clear();
        }
    }
}
