namespace WallyInterpreter.Components.Interpreter.Automaton
{
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
        public void AddState(IState<T> state)
        {
            _states.Add(state);
        }

        public T[] Alphabet()
        {
            return _alphabet;
        }

        public IState<T> CurrentState()
        {
            return _current;
        }

        public IState<T>[] Finals()
        {
            var finals = from state in _states where state.IsAccepting() select state;
            return finals.ToArray();
        }

        public bool IsDeterministic()
        {
            var count = from state in _states where state.Epsilons().Length > 0 select state;
            return count.Count() == 0;
        }

        public void Restart()
        {
            _current = _start;
        }

        public IState<T> Start()
        {
            return _start;
        }

        public IState<T>[] States()
        {
            return _states.ToArray();
        }

        public IAutomaton<T> ToDeterministic()
        {
            throw new NotImplementedException();
        }

        public void Walk(T symbol)
        {
            if (_current.IsFault()) throw new Exception("This is a Fault Node");
            if (!IsDeterministic())
                throw new Exception("This Automaton is NonDeterministic, make it Deterministic First");
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

            throw new Exception();
        }
    }
}
