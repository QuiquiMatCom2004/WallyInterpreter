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
            var G = GranmarTools.Augment(g);
            G.MakeFirstAndFollow(endmarker);

            var tools = new ParserTools();
            var canonical = tools.GetCanonicalLR0Collection(G).ToList();
            _states = canonical.Select((c, i) => (IState<string>)new State<string>($"I{i}", true, false)).ToList();

            foreach (var st in _states)
            {
                _action[st.ID()] = new Dictionary<string, ActionStruct>(StringComparer.Ordinal);
            }
            for (int i = 0; i < canonical.Count; i++)
            {
                var I_i = canonical[i];
                var stateId = _states[i].ID();
                var allsyms = G.Terminals().Concat(G.NonTerminals());
                foreach (var term in allsyms)
                {
                    var J = tools.GOTO(I_i, term, G);
                    if (J != null)
                    {
                        int j = canonical.FindIndex(c => c.ID == J.ID);
                        var sym = term.Symbol();

                        _action[stateId][sym]
                          = new ActionStruct(ParserAction.Shift, _states[j].ID());
                        _states[i].AddTransition(sym, _states[j]);
                    }
                }
            }
            for (int i = 0; i < canonical.Count; i++)
            {
                var I_i = canonical[i];
                var stateId = _states[i].ID();

                foreach (var item in I_i)
                {
                    if (item.RightPoint.Length == 0 &&
                        item.Head.Symbol() == G.StartSymbol().Symbol())
                    {
                        _action[stateId][_endmarker]
                          = new ActionStruct(ParserAction.Accept, "");
                        continue;
                    }
                    if (item.RightPoint.Length == 0 &&item.Head.Symbol() != G.StartSymbol().Symbol())
                    {
                        var head = item.Head.Symbol();
                        var rhs = item.LeftPoint.Select(x => x.Symbol()).ToArray();
                        var followA = G.Follow(item.Head);

                        foreach (var a in followA)
                        {
                            var sym = a.Symbol();
                            _action[stateId][sym] =
                                new ActionStruct(ParserAction.Reduce,"");

                            if (!_reduce.ContainsKey(stateId))
                                _reduce[stateId] = new Dictionary<string, ReduceStruct>();

                            _reduce[stateId][sym] =
                                new ReduceStruct(head, rhs);
                        }
                    }
                }
            }

            int startIdx = canonical.FindIndex(
             c => c.All(it => it.LeftPoint.Length == 0));
            _startState = _states[startIdx];
            _stateStack.Add(_startState);
            _terminals = G.Terminals().Select(t => t.Symbol()).ToList();
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
            bool reduced = false;
            do
            {
                switch (action.Action)
                {
                    case ParserAction.Accept:
                        Draw.Information.actions.Enqueue(action.Action);
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

                        var newAST = _reductionFunction[head + "-->" + string.Join("", rhs)](children.ToArray(), head);

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
