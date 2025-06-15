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
            var id = new GranmarSymbol("id",false, GranmarSymbolType.Terminal);
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
            var arrow = new GranmarSymbol("->", false, GranmarSymbolType.Terminal);
            var coma = new GranmarSymbol(",", false, GranmarSymbolType.Terminal);
            //EOL
            var EOL = new GranmarSymbol("EOL", false, GranmarSymbolType.Terminal);
            //Keys
            var goTo = new GranmarSymbol("Goto",false, GranmarSymbolType.Terminal);
            #endregion
            #region ProductionDefinition
            //Program
            var gProgram = new Granmar.Granmar(Program);
            gProgram.AddProduction(Program, new[] { StmtList });//
            //Stament List
            gProgram.AddProduction(StmtList, new[] { Statement, EOL, StmtList });//
            gProgram.AddProduction(StmtList, new[] { Statement });//
            //Statement
            gProgram.AddProduction(Statement, new[] { AssignStmt });//
            gProgram.AddProduction(Statement, new[] { ExprStmt });//
            gProgram.AddProduction(Statement, new[] { GotoStmt });//
            gProgram.AddProduction(Statement, new[] { LabelDecl });//
            //Assignacion
            gProgram.AddProduction(AssignStmt, new[] { id, arrow, ExprStmt });//
            //Label
            gProgram.AddProduction(LabelDecl, new[] { id });//
            //Goto
            gProgram.AddProduction(GotoStmt, new[] { goTo, lcorchete,LabelDecl,rcorchete, lparent,BoolExpr,rparent});//
            //Expresions
            gProgram.AddProduction(ExprStmt, new[] { BoolExpr });//
            //Booleans Expression
            gProgram.AddProduction(BoolExpr, new[] { BoolExpr, and, BoolExpr});//
            gProgram.AddProduction(BoolExpr, new[] { BoolExpr, or, BoolExpr });//
            gProgram.AddProduction(BoolExpr, new[] { not, BoolExpr });//
            gProgram.AddProduction(BoolExpr, new[] { BoolExpr, RelOP, BoolExpr });//
            gProgram.AddProduction(BoolExpr, new[] { ArithExpr });//
            //Relation Operators
            gProgram.AddProduction(RelOP, new[] {compareEqual });//
            gProgram.AddProduction(RelOP, new[] { lessEqual });//
            gProgram.AddProduction(RelOP, new[] { moreEqual });//
            gProgram.AddProduction(RelOP, new[] { less });//
            gProgram.AddProduction(RelOP, new[] { more });//
            //Aritmetic expresion
            gProgram.AddProduction(ArithExpr, new[] {Term,plus,ArithExpr });//
            gProgram.AddProduction(ArithExpr, new[] { Term, minus, ArithExpr });//
            gProgram.AddProduction(ArithExpr, new[] { Term});//
            //Term
            gProgram.AddProduction(Term, new[] {Power,mult,Term });//
            gProgram.AddProduction(Term, new[] { Power, div, Term });//
            gProgram.AddProduction(Term, new[] { Power, mod, Term });//
            gProgram.AddProduction(Term, new[] { Power});//
            //Power
            gProgram.AddProduction(Power, new[] {Factor , expo, Power });//
            gProgram.AddProduction(Power, new[] { Factor });//
            //Factor
            gProgram.AddProduction(Factor, new[] { number });//
            gProgram.AddProduction(Factor, new[] { boolean });//
            gProgram.AddProduction(Factor, new[] { str });//
            gProgram.AddProduction(Factor, new[] { id });//
            gProgram.AddProduction(Factor, new[] { FuncCall });//
            gProgram.AddProduction(Factor, new[] { lparent, ExprStmt, rparent });//
            //Function call
            gProgram.AddProduction(FuncCall, new[] { id , lparent,Args,rparent});//
            //Arguments
            gProgram.AddProduction(Args, new[] { ArgList });//
            gProgram.AddProduction(Args, new[] { epsilon });//
            gProgram.AddProduction(ArgList, new[] { ExprStmt });//
            gProgram.AddProduction(ArgList, new[] { ExprStmt , coma, ArgList});//
            #endregion
            Wally = gProgram;
        }
    }
}
