using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.LexicalAnalizer
{
    public class LexicalRule(string errorRule,Func<IToken,bool> rule) : ILexicalRule
    {
        private string _errorRule = errorRule;
        private Func<IToken, bool> _rule = rule;
        public string ErrorRule()
        {
            return _errorRule;
        }

        public Func<IToken, bool> Rule()
        {
            return _rule;
        }
    }
}
