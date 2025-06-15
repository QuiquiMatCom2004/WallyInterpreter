using WallyInterpreter.Components.Interpreter.Errors;
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

        public IError CheckRule(IToken token)
        {
            if (token.Type() == Tokentype.Garbage)
                return new Error("GarbageToken " + token.Lexeme(),token.Line(),token.Column(),ErrorType.Lexical);
            if(rules.ContainsKey(token.Type()))
            {
                foreach (var rule in rules[token.Type()])
                {
                    if (!rule.Rule()(token))
                        return new Error(rule.ErrorRule() + ": " + token.Lexeme(), token.Line(), token.Column(), ErrorType.Lexical);
                }
            }
            return null;
        }
    }
}
