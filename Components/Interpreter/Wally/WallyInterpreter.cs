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
        public WallyInterpreter() 
        {
            #region Lexing
            var lexer = new Lexer.Lexer();
            lexer.AddTokenExpresion(Tokens.Tokentype.Keyword, 0, new KeywordsGranmar().KeyGranmar);
            lexer.AddTokenExpresion(Tokens.Tokentype.Boolean, 1, new BooleanGranmar().Boolean);
            lexer.AddTokenExpresion(Tokens.Tokentype.Number, 2, new NumberGrammar().Number);
            lexer.AddTokenExpresion(Tokens.Tokentype.Operator, 5, new OperatorGrammar().Operator);
            lexer.AddTokenExpresion(Tokens.Tokentype.Identifier, 4, new IdentifierGrammar().Identifier);
            lexer.AddTokenExpresion(Tokens.Tokentype.Symbol,3,new SymbolsGranmar().Symbols);
            lexer.AddTokenExpresion(Tokens.Tokentype.String, 6, new StringGrammar().StringGranmar);
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
            var parser = new ParserSLR(new WallyGrammar().Wally, new GranmarSymbol("$", false, GranmarSymbolType.Terminal),new ASTBuilder().Build);
            //AtomicReductions
            parser.SetReduction("Program-->StmtList",ReductionFunctions.AtomicReductor);
            parser.SetReduction("Statement-->AssignStmt", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Statement-->ExprStmt", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Statement-->GotoStmt", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Statement-->LabelDecl", ReductionFunctions.AtomicReductor);
            parser.SetReduction("ExprStmt-->BoolExpr", ReductionFunctions.AtomicReductor);
            parser.SetReduction("BoolExpr-->ArithExpr", ReductionFunctions.AtomicReductor);
            parser.SetReduction("ArithExpr-->Term", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Term-->Power", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Power-->Factor", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Factor-->string", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Factor-->boolean", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Factor-->number", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Factor-->FuncCall", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Args-->ArgumentsList", ReductionFunctions.AtomicReductor);
            parser.SetReduction("Args-->epsilon", ReductionFunctions.AtomicReductor);
            //Binary Reductions
            parser.SetReduction("BoolExpr-->BoolExpr&&BoolExpr", ReductionFunctions.BinaryOperatorReduction);
            parser.SetReduction("BoolExpr-->BoolExpr||BoolExpr", ReductionFunctions.BinaryOperatorReduction);
            parser.SetReduction("BoolExpr-->BoolExprRelationOperatorBoolExpr", ReductionFunctions.BinaryOperatorReduction);
            parser.SetReduction("ArithExpr-->Term+ArithExpr", ReductionFunctions.BinaryOperatorReduction);
            parser.SetReduction("ArithExpr-->Term-ArithExpr", ReductionFunctions.BinaryOperatorReduction);
            parser.SetReduction("Term-->Power*Term", ReductionFunctions.BinaryOperatorReduction);
            parser.SetReduction("Term-->Power/Term", ReductionFunctions.BinaryOperatorReduction);
            parser.SetReduction("Term-->Power%Term", ReductionFunctions.BinaryOperatorReduction);
            parser.SetReduction("Power-->Factor**Power", ReductionFunctions.BinaryOperatorReduction);
            //Unary Reductions
            parser.SetReduction("BoolExpr-->!BoolExpr", ReductionFunctions.UnaryOperatorReduction);
            //(Expre) reduction
            parser.SetReduction("Factor-->(ExprStmt)", ReductionFunctions.BinaryOperatorReduction);
            //ListReduction
            parser.SetReduction("StmtList-->StatementEOLStmtList", (asts, symbol)=>
            {
                var head = asts[0];
                var tail = (StmtListAST)asts[2];
                var list = new List<IAST>();
                list.Add(head);
                list.AddRange(tail.statements);
                return new StmtListAST(list.ToArray(),tail.Line,tail.Column);
            }
            );
            parser.SetReduction("StmtList-->Statement",  (asts , s) =>new StmtListAST(new IAST[] { asts[0] }, asts[0].Line, asts[0].Column));
            parser.SetReduction("ArgumentsList-->ExprStmt,ArgumentsList", (asts,s) => {
                var head = asts[0];
                var tail = (StmtListAST)asts[2];
                var list = new List<IAST>();
                list.Add(head);
                list.AddRange(tail.statements);
                return new StmtListAST(list.ToArray(), tail.Line, tail.Column+1);
            });
            parser.SetReduction("ArgumentsList-->ExprStmt", (asts, s) => {
                var head = asts[0];
                var tail = asts[2];
                var list = new List<IAST>();
                list.Add(head);
                list.Add(tail);
                return new StmtListAST(list.ToArray(), tail.Line, tail.Column+1);
            });
            parser.SetReduction("FuncCall-->id(Args)", (asts, s) => {
                var name = asts[0].Symbol;
                var args = (StmtListAST)asts[2];
                return new FuncCallAST(name,args.Line,args.Column+1 ,args.statements.ToArray());
            });
            parser.SetReduction("Factor-->id", (asts, s) => {
                return new VariableAST(asts[0].Symbol, asts[0].Line, asts[0].Column, ((VariableAST)asts[0])._name);
            });
            parser.SetReduction("AssignStmt-->id->ExprStmt",(asts,s)=> {
                var name = (VariableAST)asts[0];
                var exp = asts[2];
                return new AssignationAST(name._name, exp.Line, exp.Column, exp);
            });
            parser.SetReduction("GotoStmt->Goto[LabelDecl](BoolExpr)", (asts,s)=> {
                var label = (LabelAST)asts[2];
                var cond = asts[5];
                return new GoToAST(label,cond,cond.Line,cond.Column+1);
            });
            parser.SetReduction("RelationOperator->==",(asts,s) => {
                var a = asts[0];
                a.UpdateSymbol("==");
                return a;
            });
            parser.SetReduction("RelationOperator-><=", (asts, s) => {
                var a = asts[0];
                a.UpdateSymbol("<=");
                return a;
            });
            parser.SetReduction("RelationOperator->>=", (asts, s) => {
                var a = asts[0];
                a.UpdateSymbol(">=");
                return a;
            });
            parser.SetReduction("RelationOperator-><", (asts, s) => {
                var a = asts[0];
                a.UpdateSymbol("<");
                return a;
            });
            parser.SetReduction("RelationOperator->>", (asts, s) => {
                var a = asts[0];
                a.UpdateSymbol("<");
                return a;
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
            context.DefineFunctions("IsCanvasColor", globalFunctions.IsCanvasColor);
            #endregion
            interpreter =  new Interpreter.Interpreter(lexer,parser,lexAnalizer,collector,context);
        }

        public void Execute(string code)
        {
            interpreter.Execute(code);
        }
    }
}
