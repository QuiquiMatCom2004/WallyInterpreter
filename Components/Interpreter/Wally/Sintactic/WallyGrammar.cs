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
            var ExprStmt = new GranmarSymbol("ExprStmt", false, GranmarSymbolType.NonTerminal);
            var BoolExpr = new GranmarSymbol("BoolExpr", false, GranmarSymbolType.NonTerminal);
            var ArithExpr = new GranmarSymbol("ArithExpr", false, GranmarSymbolType.NonTerminal);
            var Term = new GranmarSymbol("Term", false, GranmarSymbolType.NonTerminal);
            var Power = new GranmarSymbol("Power", false, GranmarSymbolType.NonTerminal);
            var Factor = new GranmarSymbol("Factor", false, GranmarSymbolType.NonTerminal);
            var FuncCall = new GranmarSymbol("FuncCall", false, GranmarSymbolType.NonTerminal);
            var Args = new GranmarSymbol("Args", false, GranmarSymbolType.NonTerminal);
            var ArgList = new GranmarSymbol("ArgumentsList", false, GranmarSymbolType.NonTerminal);
            var RelOP = new GranmarSymbol("RelationOperator", false, GranmarSymbolType.NonTerminal);
            #endregion
            #region Terminals
            //Types
            var id = new GranmarSymbol("id", false, GranmarSymbolType.Terminal);
            var boolean = new GranmarSymbol("boolean", false, GranmarSymbolType.Terminal);
            var str = new GranmarSymbol("string", false, GranmarSymbolType.Terminal);
            var number = new GranmarSymbol("number", false, GranmarSymbolType.Terminal);

            var epsilon = new GranmarSymbol("epsilon", true, GranmarSymbolType.Terminal);
            //Operators
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
            //Symbols
            var lparent = new GranmarSymbol("(", false, GranmarSymbolType.Terminal);
            var rparent = new GranmarSymbol(")", false, GranmarSymbolType.Terminal);
            var lcorchete = new GranmarSymbol("[", false, GranmarSymbolType.Terminal);
            var rcorchete = new GranmarSymbol("]", false, GranmarSymbolType.Terminal);
            var arrow = new GranmarSymbol("<-", false, GranmarSymbolType.Terminal);
            var coma = new GranmarSymbol(",", false, GranmarSymbolType.Terminal);
            //EOL
            var EOL = new GranmarSymbol("EOL", false, GranmarSymbolType.Terminal);
            //Keys
            var goTo = new GranmarSymbol("Goto", false, GranmarSymbolType.Terminal);
            #endregion
            #region ProductionDefinition
            var g = new Granmar.Granmar(Program);


            g.AddProduction(Program, new[] { StmtList });
            g.AddProduction(StmtList, new[] { Statement, EOL, StmtList });
            g.AddProduction(StmtList, new[] { Statement });
            g.AddProduction(Statement, new[] { LabelDecl });
            g.AddProduction(Statement, new[] { AssignStmt });
            g.AddProduction(Statement, new[] { GotoStmt });
            g.AddProduction(Statement, new[] { ExprStmt });
            g.AddProduction(LabelDecl, new[] { id });

            g.AddProduction(AssignStmt, new[] { id, arrow, ExprStmt });


            g.AddProduction(GotoStmt, new[] { goTo, lcorchete, LabelDecl, rcorchete, lparent, BoolExpr, rparent });

            g.AddProduction(ExprStmt, new[] { BoolExpr });
            g.AddProduction(BoolExpr, new[] { BoolExpr, and, BoolExpr });
            g.AddProduction(BoolExpr, new[] { BoolExpr, or, BoolExpr });
            g.AddProduction(BoolExpr, new[] { not, BoolExpr });
            g.AddProduction(BoolExpr, new[] { BoolExpr, RelOP, BoolExpr });
            g.AddProduction(BoolExpr, new[] { ArithExpr });
            g.AddProduction(RelOP, new[] { compareEqual });
            g.AddProduction(RelOP, new[] { lessEqual });
            g.AddProduction(RelOP, new[] { moreEqual });
            g.AddProduction(RelOP, new[] { less });
            g.AddProduction(RelOP, new[] { more });

            g.AddProduction(ArithExpr, new[] { Term, plus, ArithExpr });
            g.AddProduction(ArithExpr, new[] { Term, minus, ArithExpr });
            g.AddProduction(ArithExpr, new[] { Term });
            g.AddProduction(Term, new[] { Power, mult, Term });
            g.AddProduction(Term, new[] { Power, div, Term });
            g.AddProduction(Term, new[] { Power, mod, Term });
            g.AddProduction(Term, new[] { Power });
            g.AddProduction(Power, new[] { Factor, expo, Power });
            g.AddProduction(Power, new[] { Factor });

            g.AddProduction(Factor, new[] { number });
            g.AddProduction(Factor, new[] { boolean });
            g.AddProduction(Factor, new[] { str });
            g.AddProduction(Factor, new[] { id });
            g.AddProduction(Factor, new[] { FuncCall });
            g.AddProduction(Factor, new[] { lparent, ExprStmt, rparent });
            g.AddProduction(FuncCall, new[] { id, lparent, Args, rparent });
            g.AddProduction(Args, new[] { ArgList });
            g.AddProduction(Args, new[]{epsilon});

            g.AddProduction(ArgList, new[] { ExprStmt });
            g.AddProduction(ArgList, new[] { ExprStmt, coma, ArgList });

            Wally = g;
            #endregion
        }
    }
}
