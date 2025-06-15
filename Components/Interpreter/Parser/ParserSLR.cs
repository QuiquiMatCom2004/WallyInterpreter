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
        private Dictionary<string, Dictionary<string, ActionStruct>> _action = new Dictionary<string, Dictionary<string, ActionStruct>>();
        private Dictionary<string, Dictionary<string, ReduceStruct>> _reduce = new Dictionary<string, Dictionary<string, ReduceStruct>>();
        private List<IAST> _stack = new List<IAST>();
        private Func<IToken, string, IAST> _ast_engine;
        private Dictionary<string, Func<IAST[],string,IAST>> _reductionFunction = new Dictionary<string, Func<IAST[], string, IAST>>();
        private string _endmarker;
        private List<string> _terminals = new List<string>();
        public ParserSLR(IGranmar g, IGranmarSymbol endmarker,Func<IToken,string,IAST> ast_engine) {
            _endmarker = endmarker.Symbol();
            _ast_engine = ast_engine;
            ParserTools parserTools = new ParserTools();

            var G = GranmarTools.Augment(g);
            G.MakeFirstAndFollow(endmarker);
            var states = parserTools.GetCanonicalLR0Collection(G).ToList();
            int pos = -1;
            _states =states.Select((col) => {
                pos++;
                return new State<string>("I"+pos.ToString(), true,false);
            }).Cast<IState<string>>().ToList();

            int startStateIndex = states.FindIndex(collection => collection.All(item => item.LeftPoint.Length == 0));
            for(int i =0; i < states.Count(); i++) { 
                foreach(var item in states[i])
                {
                    if(item.RightPoint.Length > 0)
                    {
                        foreach(var terminal in G.Terminals())
                        {
                            var gotoResult = parserTools.GOTO(states[i], terminal, G);
                            if(gotoResult!= null)
                            {
                                var nextState = states.FindIndex(coll => coll.ID ==gotoResult.ID);
                                _states[i].AddTransition(terminal.Symbol(), _states[nextState]);
                                if (!_action.ContainsKey(_states[i].ID()))
                                {
                                    _action[_states[i].ID()] = new Dictionary<string, ActionStruct>();
                                }
                                if (_action[_states[i].ID()].TryGetValue(terminal.Symbol(), out ActionStruct actRec))
                                {
                                    if (actRec.Action != ParserAction.Shift)
                                        throw new Exception("The given grammar is not SLR(1)");
                                }
                                _action[_states[i].ID()][terminal.Symbol()] = new ActionStruct(ParserAction.Shift, _states[nextState].ID());
                            }
                        }
                    }
                    if(item.RightPoint.Length == 0 && item.Head.Symbol() != G.StartSymbol().Symbol())
                    {
                        foreach (var terminal in G.Follow(item.Head))
                        {
                            if (!_action.ContainsKey(_states[i].ID())) {
                                _action[_states[i].ID()] = new Dictionary<string, ActionStruct>();
                            }
                            if (_action[_states[i].ID()].TryGetValue(terminal.Symbol(), out ActionStruct actRec))
                            {
                                if (actRec.Action != ParserAction.Reduce)
                                    throw new Exception("The given grammar is not SLR(1)");
                            }
                            _action[_states[i].ID()][terminal.Symbol()] = new ActionStruct(ParserAction.Reduce, "");
                            if (!_reduce.ContainsKey(_states[i].ID()))
                            {
                                _reduce[_states[i].ID()] = new Dictionary<string, ReduceStruct>();
                                _reduce[_states[i].ID()][terminal.Symbol()] = new ReduceStruct(item.Head.Symbol(), item.LeftPoint.Select(s => s.Symbol()).ToArray());

                            }
                        }
                    }
                    if (item.RightPoint.Length == 0 && item.Head.Symbol() == G.StartSymbol().Symbol())
                    {
                        if(!_action.ContainsKey(_states[i].ID()))
                        {
                            _action[_states[i].ID()] = new Dictionary<string, ActionStruct>();
                            _action[_states[i].ID()][_endmarker] = new ActionStruct(ParserAction.Accept,"");
                        }
                    }
                }
            }
            for (int i = 0; i < states.Count; i++) {
                foreach (var nt in G.NonTerminals())
                {
                    var gotoresult = parserTools.GOTO(states[i], nt, G);
                    if (gotoresult != null)
                    {
                        int next_index = states.FindIndex(coll => coll.ID == gotoresult.ID);
                        _states[i].AddTransition(nt.Symbol(), _states[next_index]);

                        if (!_action.ContainsKey(nt.Symbol()))
                        {
                            _action[_states[i].ID()] = new Dictionary<string, ActionStruct>();
                        }
                        if (_action[_states[i].ID()].TryGetValue(nt.Symbol(), out ActionStruct actRec))
                        {
                            if (actRec.Action != ParserAction.Shift)
                                throw new Exception("The given grammar is not SLR(1)");
                        }
                        _action[_states[i].ID()][nt.Symbol()] = new ActionStruct(ParserAction.Shift, _states[next_index].ID());
                    } 
                }
            }
            _startState = _states[startStateIndex];
            _stateStack.Add(_startState);
            _terminals = G.Terminals().Select(s => s.Symbol()).ToList();
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
            Console.WriteLine(ast.Symbol);
            Console.WriteLine();
            foreach(var t in _action.Keys)
            {

                foreach(var v in _action[t].Keys)
                    Console.WriteLine(v);
            }
            Console.WriteLine();
            if (!_action.ContainsKey(_stateStack.Last().ID()) || !_action[_stateStack.Last().ID()].ContainsKey(ast.Symbol))
            {
                string msg = "expected ";
                foreach (var k in _action[_stateStack.Last().ID()].Keys) {
                    if (_terminals.Contains(k))
                        msg += k + ", ";
                }
                if(ast.Symbol == _endmarker)
                {
                    collector.AddError(new Error($"Unexpected EOF symbol {msg}",ast.Line,ast.Column,ErrorType.Gramatical));
                }
                else
                {
                    collector.AddError(new Error($"Unexpected Symbol {ast.Symbol},{msg}",ast.Line,ast.Column,ErrorType.Gramatical));
                }
                return;
            }
            bool symbolAccepted = false;
            while (!symbolAccepted) { 
                ActionStruct action = _action[_stateStack.Last().ID()][ast.Symbol];
                if(action.Action == ParserAction.Shift || action.Action == ParserAction.Accept)
                {
                    _stateStack.Add(_stateStack.Last().Next(ast.Symbol));
                    _stack.Add(ast);
                    symbolAccepted = true;
                }
                else if(action.Action == ParserAction.Reduce)
                {
                    ReduceStruct reduction = _reduce[_stateStack.Last().ID()][ast.Symbol];
                    string reductionID = reduction.NewSymbol + "-->" +string.Join("",reduction.Symbols);
                    List<IAST> symbolToReduce = _stack.Skip(_stack.Count() - reduction.Symbols.Count()).ToList();
                    _stack = _stack.Take(_stack.Count() - reduction.Symbols.Count()).ToList();
                    if (_reductionFunction.ContainsKey(reductionID)) { 
                        var reductor = _reductionFunction[reductionID];
                        var newAst = reductor(symbolToReduce.ToArray(), reduction.NewSymbol);
                        _stack.Add(newAst);
                        _stateStack = _stateStack.Take(_stateStack.Count() - reduction.Symbols.Count()).ToList();
                        action = _action[_stateStack.Last().ID()][newAst.Symbol];
                        _stateStack.Add(_stateStack.Last().Next(newAst.Symbol));
                    }
                    else
                    {
                        throw new Exception("Its not reduction define for " + reductionID);
                    }
                }
            }
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
    }
}
