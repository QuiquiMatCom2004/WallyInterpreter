using BlazorMonaco.Languages;
using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Wally.Sintactic
{
    public class WallyGrammar
    {
        public IGranmar Wally;
        public WallyGrammar()
        {
            #region NonTerminals
            var Program = new GranmarSymbol("Program", false, GranmarSymbolType.NonTerminal);
            var StmtList = new GranmarSymbol("StmtList", false, GranmarSymbolType.NonTerminal);
            var Statement = new GranmarSymbol("Statement", false, GranmarSymbolType.NonTerminal);
            var AssignStmt = new GranmarSymbol("AssignStmt", false, GranmarSymbolType.NonTerminal);
            var LabelDecl = new GranmarSymbol("LabelDecl", false, GranmarSymbolType.NonTerminal);
            var GotoStmt = new GranmarSymbol("GotoStmt", false, GranmarSymbolType.NonTerminal);
            var BoolExpr = new GranmarSymbol("BoolExpr", false, GranmarSymbolType.NonTerminal);
            var ArithExpr = new GranmarSymbol("ArithExpr", false, GranmarSymbolType.NonTerminal);
            var Term = new GranmarSymbol("Term", false, GranmarSymbolType.NonTerminal);
            var Power = new GranmarSymbol("Power", false, GranmarSymbolType.NonTerminal);
            var Factor = new GranmarSymbol("Factor", false, GranmarSymbolType.NonTerminal);
            var FuncCall = new GranmarSymbol("FuncCall", false, GranmarSymbolType.NonTerminal);
            var Args = new GranmarSymbol("Args", false, GranmarSymbolType.NonTerminal);
            var OrExpr = new GranmarSymbol("OrExpr", false, GranmarSymbolType.NonTerminal);
            var NotExpr = new GranmarSymbol("NotExpr", false, GranmarSymbolType.NonTerminal);
            var RelExpr = new GranmarSymbol("RelExpr", false, GranmarSymbolType.NonTerminal);
            #endregion
            #region Terminals
            var id = new GranmarSymbol("id", false, GranmarSymbolType.Terminal);
            var boolean = new GranmarSymbol("boolean", false, GranmarSymbolType.Terminal);
            var str = new GranmarSymbol("string", false, GranmarSymbolType.Terminal);
            var number = new GranmarSymbol("number", false, GranmarSymbolType.Terminal);
            var plus = new GranmarSymbol("+", false, GranmarSymbolType.Terminal);
            var minus = new GranmarSymbol("-", false, GranmarSymbolType.Terminal);
            var mult = new GranmarSymbol("*", false, GranmarSymbolType.Terminal);
            var div = new GranmarSymbol("/", false, GranmarSymbolType.Terminal);
            var mod = new GranmarSymbol("%", false, GranmarSymbolType.Terminal);
            var expo = new GranmarSymbol("**", false, GranmarSymbolType.Terminal);
            var or = new GranmarSymbol("||", false, GranmarSymbolType.Terminal);
            var and = new GranmarSymbol("&&", false, GranmarSymbolType.Terminal);
            var lessEqual = new GranmarSymbol("<=", false, GranmarSymbolType.Terminal);
            var moreEqual = new GranmarSymbol(">=", false, GranmarSymbolType.Terminal);
            var more = new GranmarSymbol(">", false, GranmarSymbolType.Terminal);
            var less = new GranmarSymbol("<", false, GranmarSymbolType.Terminal);
            var compareEqual = new GranmarSymbol("==", false, GranmarSymbolType.Terminal);
            var not = new GranmarSymbol("!", false, GranmarSymbolType.Terminal);
            var lparent = new GranmarSymbol("(", false, GranmarSymbolType.Terminal);
            var rparent = new GranmarSymbol(")", false, GranmarSymbolType.Terminal);
            var lbracket = new GranmarSymbol("[", false, GranmarSymbolType.Terminal);
            var rbracket = new GranmarSymbol("]", false, GranmarSymbolType.Terminal);
            var arrow = new GranmarSymbol("<-", false, GranmarSymbolType.Terminal);
            var coma = new GranmarSymbol(",", false, GranmarSymbolType.Terminal);
            var EOL = new GranmarSymbol("EOL", false, GranmarSymbolType.Terminal);
            var goTo = new GranmarSymbol("goto", false, GranmarSymbolType.Terminal);
            #endregion
            IGranmar g = new Granmar.Granmar(Program);
            g.AddProduction(Program, new[] { StmtList});
            g.AddProduction(StmtList, new[] { StmtList, Statement, EOL });
            g.AddProduction(StmtList, new[] { Statement, EOL });
            g.AddProduction(StmtList, new[] { StmtList, EOL });
            g.AddProduction(StmtList, new[] { EOL });
            //Statements
            g.AddProduction(Statement, new[] { AssignStmt });
            g.AddProduction(Statement, new[] { LabelDecl });
            g.AddProduction(Statement, new[] { BoolExpr });
            g.AddProduction(Statement, new[] { GotoStmt });
            //GoTo
            g.AddProduction(GotoStmt, new[] { goTo, lbracket, id, rbracket, lparent, BoolExpr, rparent });

            //asignacion
            g.AddProduction(AssignStmt, new[] { id, arrow, BoolExpr });
            //label
            g.AddProduction(LabelDecl, new[] { id });
            //FuncCall
            g.AddProduction(FuncCall, new[] { id, lparent, rparent });
            g.AddProduction(FuncCall, new[] { id, lparent, Args ,rparent });
            //Args
            g.AddProduction(Args, new[] { Args, coma, BoolExpr });
            g.AddProduction(Args, new[] { BoolExpr });
            //Expresiones
            g.AddProduction(BoolExpr, new[] { OrExpr });
            g.AddProduction(BoolExpr, new[] { OrExpr, and, BoolExpr });

            g.AddProduction(OrExpr, new[] { OrExpr ,or, NotExpr});
            g.AddProduction(OrExpr, new[] { NotExpr});

            g.AddProduction(NotExpr, new[] {not,NotExpr });
            g.AddProduction(NotExpr, new[] { RelExpr });
            g.AddProduction(RelExpr, new[] { ArithExpr, compareEqual, ArithExpr });
            g.AddProduction(RelExpr, new[] { ArithExpr, lessEqual, ArithExpr });
            g.AddProduction(RelExpr, new[] { ArithExpr, moreEqual, ArithExpr });
            g.AddProduction(RelExpr, new[] { ArithExpr, less, ArithExpr });
            g.AddProduction(RelExpr, new[] { ArithExpr, more, ArithExpr });
            g.AddProduction(RelExpr, new[] { ArithExpr });

            g.AddProduction(ArithExpr, new[] { ArithExpr, plus, Term });
            g.AddProduction(ArithExpr, new[] { ArithExpr, minus, Term });
            g.AddProduction(ArithExpr, new[] { ArithExpr, Term });
            g.AddProduction(ArithExpr, new[] { Term });

            g.AddProduction(Term, new[] { Term, mult, Power});
            g.AddProduction(Term, new[] { Term, div, Power });
            g.AddProduction(Term, new[] { Term, mod, Power });
            g.AddProduction(Term, new[] { Power });

            g.AddProduction(Power, new[] { Power, expo, Factor });
            g.AddProduction(Power, new[] { Factor });


            g.AddProduction(Factor, new[] { number });
            g.AddProduction(Factor, new[] { boolean });
            g.AddProduction(Factor, new[] { str });
            g.AddProduction(Factor, new[] { id });
            g.AddProduction(Factor, new[] { FuncCall });
            g.AddProduction(Factor, new[] { lparent, BoolExpr, rparent });

            Wally = g;
        }
    }
}
