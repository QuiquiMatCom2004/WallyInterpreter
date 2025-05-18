namespace WallyInterpreter.Components.Interpreter.Errors
{
    public class FaultNodeException : Exception
    {
        string mensaje = "This is a Fault Node";

        public FaultNodeException()
        {
            Console.WriteLine(mensaje);
        }

        public FaultNodeException(string? message) : base(message)
        {
            if (message == null) message = mensaje;
        }
    }
    public class NFA_AutomatonException:Exception
    {
        public NFA_AutomatonException()
        {
        }

        public NFA_AutomatonException(string? message) : base(message)
        {
            if (message == null) message = "This Automaton is NonDeterministic, make it Deterministic First";
        }
    }
}
