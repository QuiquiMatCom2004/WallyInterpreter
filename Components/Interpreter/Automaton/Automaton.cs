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
            AuxStructToDeterministic resultofDeterministicAux = AuxDeterministic();
            List<IState<T>> states = new List<IState<T>>();
            IState<T> initialState = null;
            foreach(var statesr in resultofDeterministicAux.states)
            {
                var new_state = MakeStateFromStates(statesr);
                if(new_state.ID() == resultofDeterministicAux.id)
                {
                    initialState = new_state;
                }
                states.Add(new_state);
            }
            foreach(var item in resultofDeterministicAux.transition)
            {
                int index = (from s in states where s.ID() == item.Key select states.IndexOf(s)).FirstOrDefault();
                foreach(var dict in item.Value)
                {
                    int index2 = (from s in states where s.ID() == dict.Value select states.IndexOf(s)).FirstOrDefault();
                    states[index].AddTransition(dict.Key , states[index2]);
                }
            }
            return new Automaton<T>(initialState, states,Alphabet());
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
            public IState<T>[][] states;
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
        private AuxStructToDeterministic AuxDeterministic()
        {
            var initial = _start.Clousure();
            var clousures = GetClousures();
            List<IState<T>[]> states = new List<IState<T>[]>();
            states.Add(initial);
            Dictionary<string, Dictionary<T, string>> transition = new Dictionary<string, Dictionary<T, string>>();
            bool state_added = true;

            while (state_added)
            {
                state_added = false;
                foreach (var state in states)
                {
                    IState<T> a_state = MakeStateFromStates(state);
                    if (!transition.ContainsKey(a_state.ID()))
                    {
                        transition[a_state.ID()] = new Dictionary<T, string>();
                    }
                    foreach (var symbol in _alphabet)
                    {
                        List<IState<T>> move = new List<IState<T>>();
                        foreach(var st in state)
                        {
                            if (!st.HasTransition(symbol))
                            {
                                continue;
                            }
                            var next = st.Next(symbol);
                            move.Add(next);
                            foreach(var e_st in clousures[next.ID()])
                            {
                                if (e_st.ID() != next.ID())
                                    move.Add(e_st);
                            }
                        }
                        if (move.Count == 0)
                            continue;
                        if(!AutomatonOperatorsAndTools<T>.SetStatesIsInSetofSetStates(states.ToArray(),move.ToArray()))
                        {
                            states.Add(move.ToArray());
                            state_added = true;
                            transition[a_state.ID()][symbol] = MakeStateFromStates(move.ToArray()).ID();
                            break;
                        }
                        else
                        {
                            transition[a_state.ID()][symbol] = MakeStateFromStates(move.ToArray()).ID();   
                        }

                    }
                    if (state_added)
                    {
                        break;
                    }
                }
            }
            AuxStructToDeterministic result;
            result.id = MakeStateFromStates(initial).ID();
            result.states = states.ToArray();
            result.transition = transition;
            return result;
        }
        private IState<T> MakeStateFromStates(IState<T>[] states)
        {
            var id = states[0].ID();
            foreach (var state in states)
            {
                id = "_" + state.ID();
            }
            var accepting = (from state in states where state.IsAccepting() select state).Count() > 0;
            var fault = (from state in states where state.IsFault() select state).Count() == states.Length;
            return new State<T> (id, accepting, fault);
        }
    }
}
