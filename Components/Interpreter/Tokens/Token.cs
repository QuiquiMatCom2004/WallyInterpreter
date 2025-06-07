namespace WallyInterpreter.Components.Interpreter.Tokens
{
    public class Token : IToken
    {
        private int _column;
        private int _line;
        private string _text;
        private Tokentype _type;
        public Token(int line, int column, string text, Tokentype type)
        {
            _column = column;
            _line = line;
            _text = text;
            _type = type;
        }

        public int Column()
        {
            return _column;
        }

        public string Lexeme()
        {
            return _text;
        }

        public int Line()
        {
            return _line;
        }

        public Tokentype Type()
        {
            return _type;
        }
        public override string ToString() {
            return $"Lexem:{_text}, Type:{_type}, Line:{_line}, Column:{_column}";
        }
    }
}
