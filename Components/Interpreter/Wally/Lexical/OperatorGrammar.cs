using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Lexical
{
    public class OperatorGrammar
    {
        public IGranmar Operator;
        public OperatorGrammar()
        {
            var op = new string[]
            {
                "+",
                "-",
                "*",
                "/",
                "**",
                "%",
                "||",
                "&&",
                "==",
                ">=",
                "<=",
                ">",
                "<",
                "!"
            };
            Operator =  GranmarTools.GetWordsGrammar(op);
        }
    }
}
