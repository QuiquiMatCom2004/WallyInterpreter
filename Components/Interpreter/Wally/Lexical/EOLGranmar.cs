using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Lexical
{
    public class EOLGranmar
    {
        public IGranmar granmar;
        public EOLGranmar()
        {
            granmar = GranmarTools.GetWordsGrammar(new[] {"\n"} );
        }
    }
}
