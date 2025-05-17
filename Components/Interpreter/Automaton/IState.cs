namespace WallyInterpreter.Components.Interpreter.Automaton
{
    public interface IState<T>
    {
        string ID();
        bool IsAccepting();
        bool IsFault();
        bool HasTransition(T symbol);
        IState<T>[] Epsilons();
        IState<T> Next(T symbol);
        void AddTransition(T symbol,IState<T> state);

        void AddEpsilon(IState<T> state);

        IState<T>[] Clousure();
    }
}
