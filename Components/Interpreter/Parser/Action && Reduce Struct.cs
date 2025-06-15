namespace WallyInterpreter.Components.Interpreter.Parser
{
    public struct ActionStruct(ParserAction action, string nextState)
    {
        public ParserAction Action= action;
        public string NextState = nextState;
    }
    public struct ReduceStruct(string newsymbol, string[]symbols)
    {
        public string NewSymbol = newsymbol;
        public List<string> Symbols= symbols.ToList();
    }
}
