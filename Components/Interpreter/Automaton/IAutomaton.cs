namespace WallyInterpreter.Components.Interpreter.Automaton
{
    public interface IAutomaton<T>
    {
        IState<T> Start();
        IState<T>[] Finals();
        IState<T>[] States();
        bool IsDeterministic();
        IAutomaton<T> ToDeterministic();
        void Walk(T symbol);
        IState<T> CurrentState();
        void AddState(IState<T> state);

        T[] Alphabet();
        void Restart();
    }
}
