using Microsoft.VisualBasic;
using WallyInterpreter.Components.Draw;
using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Lexer;
using WallyInterpreter.Components.Interpreter.LexicalAnalizer;
using WallyInterpreter.Components.Interpreter.Parser;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Tokens;
using WallyInterpreter.Components.Services;

namespace WallyInterpreter.Components.Interpreter.Interpreter
{
    public class Interpreter(ILexer lexer,IParserSLR parser, ILexicalAnalizer lanalize, IErrorColector err, IContext context) : IInterpreter
    {
        private ILexer _lexer = lexer;
        private IParserSLR _parser = parser;
        private ILexicalAnalizer _lexicalAnalizer = lanalize;
        private IErrorColector _errorColector = err;
        private IContext _context = context;

        public void Execute(string code)
        {
           _lexer.LoadCode(code);
            _parser.Reset();
            while (_lexer.Next())
            {
                var err = _lexicalAnalizer.CheckRule(_lexer.Current());
                if (err != null) { 
                    _errorColector.AddError(err);
                }
                Draw.Information.tokens.Add(_lexer.Current());
                _parser.Parse(_lexer.Current(), _errorColector);
            }
            _parser.Parse(new Token(0, 0, "\n", Tokentype.EOL),_errorColector);
            _parser.Parse(new Token(_lexer.Current().Line()+1,0,_parser.EndMarker(),Tokentype.EOF),_errorColector);
            var codeResult = _parser.GetAST().Eval(_context, _errorColector);
            _parser.Reset();
            int garbageTokens = 0;
            foreach(var err in _errorColector.GetErrors())
            {
                if(err.Message == "GarbageToken \r")
                {
                    garbageTokens++;
                    continue;
                }
                Draw.Information.errors.Add(err);  
            }
            if (_errorColector.GetErrors().Length == garbageTokens)
            {
                ConfigurationLog configurationLog = new ConfigurationLog();
                configurationLog.LogInformation($"Codigo Interpretado con exito {codeResult}");
            }
            else
            {
                foreach (var erros in _errorColector.GetErrors())
                {
                    Console.WriteLine(erros.Message);
                }
                throw new Exception("Interpretacion Process Failure");
            }
        }
    }
}
