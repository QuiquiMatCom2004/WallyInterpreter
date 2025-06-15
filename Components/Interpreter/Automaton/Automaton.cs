using WallyInterpreter.Components.Interpreter.Errors;

namespace WallyInterpreter.Components.Interpreter.Automaton
{
    /// <summary>
    /// This object represent a state machine where the symbols type correspond to alphabet
    /// </summary>
    /// <typeparam name="T">Type of symbols</typeparam>
    public class Automaton<T> : IAutomaton<T>
    {
        private IState<T> _start;
        private IState<T> _current;
        private List<IState<T>> _states;
        private T[] _alphabet;

        public Automaton(IState<T> start, List<IState<T>> states,T[] alphabet) { 
            _start = start;
            _current = start;
            _states = states;
            _alphabet = alphabet;
        }
        /// <summary>
        /// Add new state to machine
        /// </summary>
        /// <param name="state">the state</param>
        public void AddState(IState<T> state)
        {
            _states.Add(state);
        }
        /// <summary>
        /// return the alphabeth of this automaton
        /// the alphabeth represent a set of symbols
        /// </summary>
        /// <returns>Alphabet Generic Type</returns>
        public T[] Alphabet()
        {
            return _alphabet;
        }
        /// <summary>
        /// Return current state i reading
        /// </summary>
        /// <returns>Actual State</returns>
        public IState<T> CurrentState()
        {
            return _current;
        }
        /// <summary>
        /// Returns a list of finals state the machine
        /// </summary>
        /// <returns></returns>
        public IState<T>[] Finals()
        {
            var finals = from state in _states where state.IsAccepting() select state;
            return finals.ToArray();
        }
        /// <summary>
        /// Verify if The automaton is NFA or DFA
        /// </summary>
        /// <returns>True: DFA, False: NFA</returns>
        public bool IsDeterministic()
        {
            var count = from state in _states where state.Epsilons().Length > 0 select state;
            return count.Count() == 0;
        }
        /// <summary>
        /// Restart the automaton to the initial state
        /// </summary>
        public void Restart()
        {
            _current = _start;
        }
        /// <summary>
        /// Give the initial state
        /// </summary>
        /// <returns>initial state</returns>
        public IState<T> Start()
        {
            return _start;
        }
        /// <summary>
        /// Return the set of States contains in machine
        /// </summary>
        /// <returns>All States</returns>
        public IState<T>[] States()
        {
            return _states.ToArray();
        }
        /// <summary>
        /// Transform one NFA in DFA
        /// </summary>
        /// <returns>DFA Automaton</returns>
        public IAutomaton<T> ToDeterministic()
        {
            var resultofDeterministicAux = AuxDeterministic();
            var dfaStates = resultofDeterministicAux.states.Select(set => MakeCompositeState(set)).ToDictionary(s=>s.ID(),s => s);
            foreach(var t in resultofDeterministicAux.transition)
            {
                var f = dfaStates[t.Key];
                foreach(var v in t.Value)
                {
                    f.AddTransition(v.Key, dfaStates[v.Value]);
                }
            }

            return new Automaton<T>(dfaStates[resultofDeterministicAux.id], (dfaStates.Values).ToList(),Alphabet());
        }
        /// <summary>
        /// Change the actual state according to read symbol
        /// </summary>
        /// <param name="symbol">Symbol to read</param>
        /// <exception cref="FaultNodeException">Lauch this exception if you try to realize any action in the faul node</exception>
        /// <exception cref="NFA_AutomatonException">If you try move to the next node in a NFA automaton</exception>
        public void Walk(T symbol)
        {
            if (_current.IsFault()) throw new FaultNodeException();
            if (!IsDeterministic())
                throw new NFA_AutomatonException();
            _current = _current.Next(symbol);
        }

        private struct AuxStructToDeterministic
        {
            public string id;
            public List<HashSet<IState<T>>> states;
            public Dictionary<string, Dictionary<T, string>> transition;
        }
        private Dictionary<string, IState<T>[]> GetClousures()
        {
            Dictionary<string, IState<T>[]> result = new Dictionary<string, IState<T>[]>();
            foreach (var state in _states)
            {
                result[state.ID()] = state.Clousure();
            }
            return result;
        }
        private IState<T> MakeCompositeState(HashSet<IState<T>> set)
        {
            var id = ComputedId(set);
            bool isAccepting = set.Any(s => s.IsAccepting());
            bool isFault= set.All(s => s.IsFault());
            return new State<T>(id,isAccepting, isFault);
        }
        private string ComputedId(HashSet<IState<T>> set)
        {
            var sortedId= set.Select(s => s.ID()).OrderBy(id => id , StringComparer.Ordinal).ToList();
            return string.Join(":", sortedId);
        }

        private AuxStructToDeterministic AuxDeterministic()
        {
            return AuxDeterministic(new StateComparer());
        }

        private AuxStructToDeterministic AuxDeterministic(StateComparer stateComparer)
        {
            var initialSet = new HashSet<IState<T>>(_start.Clousure(),new StateComparer());
            var cola = new Queue<HashSet<IState<T>>>();
            cola.Enqueue(initialSet);
            var see = new Dictionary<string, HashSet<IState<T>>>();
            var transition = new Dictionary<string, Dictionary<T, string>>();

            var initialId = ComputedId(initialSet);
            see[initialId] = initialSet;
            while (cola.Count > 0)
            {
                var currentSet = cola.Dequeue();
                var currentId = ComputedId(currentSet);
                transition[currentId] = new Dictionary<T, string>();
                foreach (var symbol in _alphabet)
                {
                    HashSet<IState<T>> moveSet = new HashSet<IState<T>>((IEqualityComparer<IState<T>>?)new StateComparer());
                    foreach( var st in currentSet)
                    {
                        if (!st.HasTransition(symbol))
                            continue;
                        var next = st.Next(symbol);
                        moveSet.Add(next);
                        foreach(var eps in GetClousures()[next.ID()])
                        {
                            moveSet.Add(eps);
                        }
                    }
                    if (moveSet.Count == 0)
                        continue;
                    var moveId = ComputedId(moveSet);
                    transition[currentId][symbol] = moveId;
                    if(!see.ContainsKey(moveId))
                    {
                        see[moveId] = moveSet;
                        cola.Enqueue(moveSet);
                    }
                }
            }
            return new()
            {
                id = initialId
                ,
                transition = transition
                ,
                states = see.Values.ToList()
            };
        }
        private IState<T> MakeStateFromStates(IState<T>[] states)
        {
            var id = states[0].ID();
            foreach (var state in states)
            {
                id = "_" + state.ID();
            }
            var accepting = states.ToList().FindAll(s => s.IsAccepting()).Count() > 0;
            var fault = states.ToList().FindAll(s=>s.IsFault()).Count() == states.Length;
            return new State<T> (id, accepting, fault);
        }
        public override string ToString()
        {
            var S = () => {
                string s = "";
                foreach (var state in _states)
                {
                    s += " , " + state.ID();
                }
                return s;
            };
            return $"start state: {_start.ID()} , {_alphabet.ToString()} " + S();
        }
        private class StateComparer : IEqualityComparer<IState<T>>
        {
            public bool Equals(IState<T> x, IState<T> y)
              => x is not null
                 && y is not null
                 && x.ID() == y.ID();

            public int GetHashCode(IState<T> obj)
              => obj.ID().GetHashCode();
        }
    }
}
