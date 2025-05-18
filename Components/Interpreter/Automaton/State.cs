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
        /// <summary>
        /// Add a epsilon transition pointing the state getted
        /// </summary>
        /// <param name="state">the state to link</param>
        public void AddEpsilon(IState<T> state)
        {
            _epsilon.Add(state);
        }
        /// <summary>
        /// Add transition pointing a state when you read the symbol
        /// </summary>
        /// <param name="symbol">State to move</param>
        /// <param name="state"> Symbol to read</param>
        public void AddTransition(T symbol, IState<T> state)
        {
            _transition[symbol] = state;
        }
        /// <summary>
        /// return all the states able to arrive with a epsilon transition more this state
        /// </summary>
        /// <returns>Array of states</returns>
        public IState<T>[] Clousure()
        {
            List<IState<T>> clousure = new List<IState<T>>() { this};
            foreach(var item in _epsilon)
            {
                clousure.Add(item);
            }
            return clousure.ToArray();
        }
        /// <summary>
        /// return all the states able to arrive with a epsilon transition
        /// </summary>
        /// <returns></returns>
        public IState<T>[] Epsilons()
        {
            if (_fault)
                return new IState<T>[] {};
            return _epsilon.ToArray();
        }
        /// <summary>
        /// Says if this state have a transition for the symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HasTransition(T symbol)
        {
            if(_fault)return false;
            if(_transition.ContainsKey(symbol))
                return true;
            return false;
        }
        /// <summary>
        /// return the id 
        /// </summary>
        /// <returns></returns>
        public string ID()
        {
            return _id;
        }
        /// <summary>
        /// return if is final state
        /// </summary>
        /// <returns>true is final</returns>
        public bool IsAccepting()
        {
            return _accepting;
        }
        /// <summary>
        /// return if is faul state
        /// </summary>
        /// <returns>true is fault</returns>
        public bool IsFault()
        {
            return _fault;
        }
        /// <summary>
        /// return the next state for the symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public IState<T> Next(T symbol)
        {
            if (_fault) return this;
            if (HasTransition(symbol)) return _transition[symbol];
            return new State<T>("Fault", false, true);
        }
    }
}
