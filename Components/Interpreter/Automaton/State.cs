using Microsoft.Extensions.Primitives;

namespace WallyInterpreter.Components.Interpreter.Automaton
{
    public class State<T> : IState<T>
    {
        private string _id;
        private Dictionary<T, IState<T>> _transition;
        private List<IState<T>> _epsilon;
        private bool _accepting;
        private bool _fault;

        public State(string id, bool accepting, bool fault)
        {
            _id = id;
            _transition = new Dictionary<T, IState<T>>();
            _epsilon = new List<IState<T>>();
            _accepting = accepting;
            _fault = fault;
        }

        public void AddEpsilon(IState<T> state)
        {
            _epsilon.Add(state);
        }

        public void AddTransition(T symbol, IState<T> state)
        {
            _transition[symbol] = state;
        }

        public IState<T>[] Clousure()
        {
            List<IState<T>> clousure = new List<IState<T>>() { this};
            foreach(var item in _epsilon)
            {
                clousure.Add(item);
            }
            return clousure.ToArray();
        }

        public IState<T>[] Epsilons()
        {
            if (_fault)
                return new IState<T>[] {};
            return _epsilon.ToArray();
        }

        public bool HasTransition(T symbol)
        {
            if(_fault)return false;
            if(_transition.ContainsKey(symbol))
                return true;
            return false;
        }

        public string ID()
        {
            return _id;
        }

        public bool IsAccepting()
        {
            return _accepting;
        }

        public bool IsFault()
        {
            return _fault;
        }

        public IState<T> Next(T symbol)
        {
            if (_fault) return this;
            if (HasTransition(symbol)) return _transition[symbol];
            return new State<T>("Fault", false, true);
        }
    }
}
