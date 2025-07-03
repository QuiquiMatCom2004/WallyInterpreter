using System.Reflection.Metadata.Ecma335;
using WallyInterpreter.Components.Draw;
using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Granmar;
using WallyInterpreter.Components.Interpreter.Interpreter;
using WallyInterpreter.Components.Interpreter.LexicalAnalizer;
using WallyInterpreter.Components.Interpreter.Parser;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Wally.Lexical;
using WallyInterpreter.Components.Interpreter.Wally.Sintactic;

namespace WallyInterpreter.Components.Interpreter.Wally
{
    public class WallyInterpreter : IInterpreter
    {
        private IInterpreter interpreter;
        public CanvasBuff canvas;
        public WallyInterpreter(int CanvasSize) 
        {
            #region Lexing
            var lexer = new Lexer.Lexer();
            lexer.AddTokenExpresion(Tokens.Tokentype.Keyword, 0, new KeywordsGranmar().KeyGranmar);
            lexer.AddTokenExpresion(Tokens.Tokentype.Boolean, 1, new BooleanGranmar().Boolean);
            lexer.AddTokenExpresion(Tokens.Tokentype.Number, 2, new NumberGrammar().Number);
            lexer.AddTokenExpresion(Tokens.Tokentype.String,3 , new StringGrammar().StringGranmar);
            lexer.AddTokenExpresion(Tokens.Tokentype.Symbol,4,new SymbolsGranmar().Symbols);
            lexer.AddTokenExpresion(Tokens.Tokentype.Operator, 5, new OperatorGrammar().Operator);
            lexer.AddTokenExpresion(Tokens.Tokentype.Identifier, 6, new IdentifierGrammar().Identifier);
            lexer.AddTokenExpresion(Tokens.Tokentype.EOL, 7, new EOLGranmar().granmar);

            var identRule = new LexicalRule("Variables must be start with a letter", (token) => { 
                var c = token.Lexeme()[0];
                return !char.IsDigit(c) && c != '_';
            });

            var numRule = new LexicalRule("Numbers must not have invalid leading zeros",
                t =>
                {
                    var txt = t.Lexeme();
                    if (txt.Length == 1) return true;
                    if ((txt[0] == '+' || txt[0] == '-') && txt.Length == 2)
                        return true;
                    if (txt[0] == '0' && txt[1] != '.')
                        return false;
                    return true;
                });
            var lexAnalizer = new LexicalAnalizer.LexicalAnalizer();
            lexAnalizer.AddRule(Tokens.Tokentype.Identifier, identRule);
            lexAnalizer.AddRule(Tokens.Tokentype.Number, numRule);
            #endregion
            #region Parsing
            var collector = new ErrorColector();
            var parser = new ParserSLR0 (new WallyGrammar().Wally, new GranmarSymbol("$", false, GranmarSymbolType.Terminal),new ASTBuilder().Build);
            /*parser.SetReduction("F-->id", ReductionFunctions.AtomicReductor);
            parser.SetReduction("F-->( E )", ReductionFunctions.InBettewnExtractorReduction);
            parser.SetReduction("E-->E + T", (asts, s) =>
            {
                var builder = new ASTBuilder();
                var bin = new BinaryAST(s, asts[1].Line, asts[1].Column, builder.BinaryOperators["+"]);
                bin.Left = asts[0];
                bin.Right = asts[2];
                return bin;
            });
            parser.SetReduction("T-->T * F", (asts, s) =>
            {
                var builder = new ASTBuilder();
                var bin = new BinaryAST(s, asts[1].Line, asts[1].Column, builder.BinaryOperators["*"]);
                bin.Left = asts[0];
                bin.Right = asts[2];
                return bin;
            });
            parser.SetReduction("T-->F", ReductionFunctions.AtomicReductor);
            parser.SetReduction("E-->T", ReductionFunctions.AtomicReductor);*/

            //AtomicReductions
            parser.SetReduction("Program-->StmtList", (asts, s) =>
            {
                return new ProgramAST(s, asts[0].Line, asts[0].Column, asts[0]);
            });
            parser.SetReduction("StmtList-->StmtList Statement EOL", ReductionFunctions.StmtListReduction);
            parser.SetReduction("StmtList-->StmtList EOL",ReductionFunctions.AtomicReductor);
            parser.SetReduction("StmtList-->EOL",ReductionFunctions.AtomicReductor);
            parser.SetReduction("StmtList-->Statement EOL", (asts,s)=> new StmtListAST(s,new[] { asts[0] }, asts[0].Line, asts[0].Column));
            parser.SetReduction("Statement-->AssignStmt", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Statement-->BoolExpr", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Statement-->GotoStmt", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Statement-->LabelDecl", ReductionFunctions.AtomicReductor);
            parser.SetReduction("AssignStmt-->id <- BoolExpr", ReductionFunctions.AssignationReductor);
            parser.SetReduction("GotoStmt-->goto [ id ] ( BoolExpr )", ReductionFunctions.GotoReductor);
            parser.SetReduction("LabelDecl-->id", ReductionFunctions.LabelReductor);
            parser.SetReduction("BoolExpr-->OrExpr",ReductionFunctions.AtomicReductor);
            parser.SetReduction("BoolExpr-->OrExpr && BoolExpr",ReductionFunctions.BinaryOperator);
            parser.SetReduction("OrExpr-->OrExpr || NotExpr",ReductionFunctions.BinaryOperator);
            parser.SetReduction("OrExpr-->NotExpr",ReductionFunctions.AtomicReductor);
            parser.SetReduction("NotExpr-->! NotExpr",ReductionFunctions.UnaryOperatorReduction);
            parser.SetReduction("NotExpr-->RelExpr",ReductionFunctions.AtomicReductor);
            parser.SetReduction("RelExpr-->ArithExpr > ArithExpr",ReductionFunctions.BinaryOperator);
            parser.SetReduction("RelExpr-->ArithExpr < ArithExpr",ReductionFunctions.BinaryOperator);
            parser.SetReduction("RelExpr-->ArithExpr == ArithExpr",ReductionFunctions.BinaryOperator);
            parser.SetReduction("RelExpr-->ArithExpr >= ArithExpr",ReductionFunctions.BinaryOperator);
            parser.SetReduction("RelExpr-->ArithExpr <= ArithExpr",ReductionFunctions.BinaryOperator);
            parser.SetReduction("RelExpr-->ArithExpr", ReductionFunctions.AtomicReductor);
            parser.SetReduction("ArithExpr-->ArithExpr + Term",ReductionFunctions.BinaryOperator);
            parser.SetReduction("ArithExpr-->ArithExpr - Term",ReductionFunctions.BinaryOperator);
            parser.SetReduction("ArithExpr-->ArithExpr Term", (asts, s) => {
                var b = new ASTBuilder();
                var bin = new BinaryAST(s, asts[1].Line, asts[1].Column, b.BinaryOperators["+"]);
                bin.Left = asts[0];
                bin.Right = asts[1];
                return bin;
                });
            parser.SetReduction("ArithExpr-->Term",ReductionFunctions.AtomicReductor);
            parser.SetReduction("Term-->Term * Power",ReductionFunctions.BinaryOperator);
            parser.SetReduction("Term-->Term / Power",ReductionFunctions.BinaryOperator);
            parser.SetReduction("Term-->Term % Power",ReductionFunctions.BinaryOperator);
            parser.SetReduction("Term-->Power",ReductionFunctions.AtomicReductor);
            parser.SetReduction("Power-->Power ** Factor",ReductionFunctions.BinaryOperator);
            parser.SetReduction("Power-->Factor",ReductionFunctions.AtomicReductor);
            parser.SetReduction("Factor-->id",ReductionFunctions.VariableReductor);
            parser.SetReduction("Factor-->boolean",ReductionFunctions.AtomicReductor);
            parser.SetReduction("Factor-->string",ReductionFunctions.AtomicReductor);
            parser.SetReduction("Factor-->number",ReductionFunctions.AtomicReductor);
            parser.SetReduction("Factor-->FuncCall",ReductionFunctions.AtomicReductor);
            parser.SetReduction("Factor-->( BoolExpr )",ReductionFunctions.InBettewnExtractorReduction);
            parser.SetReduction("FuncCall-->id ( )",ReductionFunctions.FunctionCallReductor);
            parser.SetReduction("FuncCall-->id ( Args )",ReductionFunctions.FunctionCallReductor);
            parser.SetReduction("Args-->Args , BoolExpr",ReductionFunctions.ArgsReductor);
            parser.SetReduction("Args-->BoolExpr", (args, s) => {
                return new StmtListAST(s, new IAST[] { args[0] }, args[0].Line, args[0].Column);
            });


            #endregion
            #region GlobalContext
            IContext context = new Context();
            GlobalFunctions globalFunctions = new GlobalFunctions();
            context.DefineFunctions("Spawn", globalFunctions.Spawn);
            context.DefineFunctions("Color", globalFunctions.Color);
            context.DefineFunctions("Size", globalFunctions.Size);
            context.DefineFunctions("DrawLine", globalFunctions.DrawLine);
            context.DefineFunctions("Fill",globalFunctions.Fill);
            context.DefineFunctions("DrawCircle", globalFunctions.DrawCircle);
            context.DefineFunctions("DrawRectangle", globalFunctions.DrawRectangle);
            context.DefineFunctions("GetActualX", globalFunctions.GetActualX);
            context.DefineFunctions("GetActualY", globalFunctions.GetActualY);
            context.DefineFunctions("GetCanvasSize", globalFunctions.GetCanvasSize);
            context.DefineFunctions("GetColorCount", globalFunctions.GetColorCount);
            context.DefineFunctions("IsBrushSize", globalFunctions.IsBrushSize);
            context.DefineFunctions("IsBrushColor", globalFunctions.IsBrushColor);
            context.DefineFunctions("IsCanvasColor", globalFunctions.IsCanvasColor);
            canvas = new CanvasBuff(CanvasSize);
            context.SetVariables("Canvas",canvas);
            #endregion
            interpreter =  new Interpreter.Interpreter(lexer,parser,lexAnalizer,collector,context);
        }

        public void Execute(string code)
        {
            interpreter.Execute(code + "$");
        }
    }
}
