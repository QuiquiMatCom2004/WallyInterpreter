using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Lexical
{
    public class BooleanGranmar
    {
        public IGranmar Boolean { get; set; }
        public BooleanGranmar()
        {
            string[] literals  = new string[] {
                "True",
                "False"
            };
            Boolean = GranmarTools.GetWordsGrammar(literals);
        }
    }
}
