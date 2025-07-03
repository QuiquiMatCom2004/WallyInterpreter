using System.Runtime.CompilerServices;

namespace WallyInterpreter.Components.Interpreter.Tokens
{
    public class ExtractorToken : IExtractorToken
    {
        private Dictionary<int, Tokentype> _priority;
        private int[] _priorityOrder;

        public ExtractorToken(Dictionary<int,Tokentype> priority) { 
            _priority = priority;
            _priorityOrder = priority.Keys.ToArray();
            _priorityOrder.ToList().Sort((a, b) => { 
                if (a < b) return -1; 
                if(a > b) return 1;
                return 0;
            });
        }
        public IToken GetToken(Tokentype[] tokentypes, string text, int line, int column)
        {
            Console.WriteLine(text);
            foreach (var p in _priorityOrder)
            {
                var expectedTokenType = _priority[p];
                if (tokentypes.Contains(expectedTokenType))
                {
                    Console.WriteLine(expectedTokenType);

                    return new Token(line, column, text, expectedTokenType);
                }
                
            }
            return new Token(line, column, text, Tokentype.Garbage);
        }
    }
}
