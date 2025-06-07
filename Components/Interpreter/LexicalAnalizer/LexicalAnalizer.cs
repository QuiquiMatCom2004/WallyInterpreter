using WallyInterpreter.Components.Interpreter.Tokens;

namespace WallyInterpreter.Components.Interpreter.LexicalAnalizer
{
    public class LexicalAnalizer : ILexicalAnalizer
    {
        private Dictionary<Tokentype, List<ILexicalRule>> rules;
        public LexicalAnalizer() { rules = new Dictionary<Tokentype, List<ILexicalRule>>(); }
        public void AddRule(Tokentype token, ILexicalRule rule)
        {
            if (rules.ContainsKey(token))
                rules[token].Add(rule);
            else
            {
                rules[token] = new List<ILexicalRule>();
                rules[token].Add(rule);
            }   
        }

        public void CheckRule(IToken token)
        {
            if (token.Type() == Tokentype.Garbage)
                throw new Exception("GarbageToken " + token.ToString());
            if(rules.ContainsKey(token.Type()))
            {
                foreach (var rule in rules[token.Type()])
                {
                    if (!rule.Rule()(token))
                        throw new Exception(rule.ErrorRule() + " : " + token.ToString());
                }
            }
        }
    }
}
