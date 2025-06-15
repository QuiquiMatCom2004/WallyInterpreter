using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Lexical
{
    public class KeywordsGranmar
    {
        public IGranmar KeyGranmar { get; set; }
        public KeywordsGranmar() {
            string[] keys = new string[] {
            "goto"
            };
            KeyGranmar = GranmarTools.GetWordsGrammar(keys);
        }
    }
}
