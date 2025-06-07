using Microsoft.AspNetCore.DataProtection.KeyManagement;
using WallyInterpreter.Components.Interpreter.Automaton;
using WallyInterpreter.Components.Interpreter.Granmar;
using WallyInterpreter.Components.Interpreter.Regex;
using WallyInterpreter.Components.Interpreter.Tokens;
using WallyInterpreter.Components.Services;

namespace WallyInterpreter.Components.Interpreter.Lexer
{
    public class Lexer : ILexer
    {
        private ConfigurationLog log;
        private Dictionary<Tokentype, IGranmar> _tokensGranmar;
        private string _code;
        private string _textReaded;
        private int _textPointer;
        private Dictionary<IAutomaton<char> , Tokentype>_automaton;
        private IExtractorToken _extractorToken;
        private Dictionary<int, Tokentype> _prioritys;
        private IToken _currentToken;
        private int _line;
        private int _column;

        public Lexer() {
            _tokensGranmar = new Dictionary<Tokentype, IGranmar>();
            _automaton = new Dictionary<IAutomaton<char>, Tokentype> ();
            _prioritys = new Dictionary<int, Tokentype>();
            _line = 1;
            _column = 0;
            _textPointer = 0;
            _extractorToken = null;
            _code = "";
            _textReaded = "";
        }
        public void AddTokenExpresion(Tokentype type, int priority, IGranmar re)
        {
            var save1 = new Dictionary<Tokentype, IGranmar>(_tokensGranmar);
            var save2 = new Dictionary<int,Tokentype>(_prioritys);
            _tokensGranmar[type] = re;
            _prioritys[priority] = type;
            _automaton = new Dictionary<IAutomaton<char>, Tokentype>();
            try
            {
                BuildAutomatons();
            }
            catch (Exception e) {
                log.LogInformation(e.Message);
                _tokensGranmar = save1;
                _prioritys = save2;
            }
        }

        public IToken Current()
        {
            return _currentToken;
        }

        public void LoadCode(string code)
        {
            _textPointer = 0;
            _line = 1;
            _column = 1;
            _textReaded = "";
            _code = code;
        }

        public bool Next()
        {
            //Reiniciar automatas
            RestartAutomatons();
            //Definir el extractor de tokens
            _extractorToken = new ExtractorToken(_prioritys);
            bool walked = false;
            List<Tokentype> lastTokentypes = new List<Tokentype>();
            do
            {
                List<Tokentype> posibleTokentype = new List<Tokentype>();
                foreach(var aut in _automaton)
                {
                    try
                    {
                        aut.Key.Walk(_code[_textPointer]);
                        walked = true;
                        if (aut.Key.CurrentState().IsFault())
                            walked = false;
                    }catch { walked = false; }
                    if (walked && aut.Key.CurrentState().IsAccepting())
                        posibleTokentype.Add(aut.Value);
                }
                if (posibleTokentype.Count > 0) walked = true;

                if (!walked)
                {
                    if (_textPointer == 0)
                    {
                        if (_code[_textPointer] != ' ' && _code[_textPointer] != '\n')
                        {
                            _currentToken = _extractorToken.GetToken(lastTokentypes.ToArray(), _code[0].ToString(), _line, _column);
                        }
                        else if (_code[_textPointer] == '\n')
                        {
                            _column = 1;
                            _line++;
                        }
                        else
                        {
                            _column++;
                        }
                        _code = _code.Substring(1);
                        walked = true;
                    }
                    else
                    {
                        _currentToken = _extractorToken.GetToken(lastTokentypes.ToArray(), _textReaded, _line, _column);
                        _code = _code.Substring(_textPointer);
                        _column += _textPointer;
                        _textPointer = 0;
                        _textReaded = "";
                    }
                    RestartAutomatons();
                }
                else
                {
                    if (_code[_textPointer] =='\n')
                    {
                        _column = 1;
                        _line++;
                    }
                    lastTokentypes = posibleTokentype;
                    _textReaded += _code[_textPointer];
                    _textPointer++;
                    if(_textPointer == _code.Length)
                    {
                        walked = false;
                    }
                }

            }
            while (walked && _code.Length>0);
            return _code.Length > 0;
        }
        private void BuildAutomatons()
        {
            IRegularExpresion regex = new RegularExpresion();
            foreach (var item in _tokensGranmar)
            {
                IAutomaton<char> aut = regex.Regex(item.Value);
                _automaton.Add(aut.ToDeterministic(),item.Key);
            }
        }
        private void RestartAutomatons()
        {
            foreach (var aut in _automaton)
            {
                aut.Key.Restart();
            }
        }
    }
}
