using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Wally.Lexical;

namespace WallyInterpreter.Components.Interpreter.Wally.Sintactic
{
    public static class ReductionFunctions
    {
        public static IAST BinaryOperatorReduction(IAST[] asts,string newSymbol)
        {
            var builder = new ASTBuilder();
            return new BinaryAST(asts[1].Symbol, asts[1].Line, asts[1].Column, builder.BinaryOperators[asts[1].Symbol], asts[0], asts[2]);
        }
        public static IAST UnaryOperatorReduction(IAST[] asts,string newSymbol)
        {
            var builder = new ASTBuilder();
            return new UnaryAST(asts[0].Symbol, asts[0].Line, asts[0].Column, builder.UnaryOperators[asts[0].Symbol], asts[1]);
        }
        public static IAST InBettewnExtractorReduction(IAST[] asts,string newSymbol)
        {
            var inner = asts[1];
            inner.UpdateSymbol(newSymbol);
            return inner;
        }
        public static IAST AtomicReductor(IAST[] asts,string newSymbol)
        {
            var atom = asts[0];
            atom.UpdateSymbol(newSymbol);
            return atom;
        }
        public static IAST AssignationReductor(IAST[] asts,string newSymbol)
        {
            var VarName = asts[0];
            var Exp = asts[2];

            return new AssignationAST(VarName.Symbol, asts[2].Line, asts[2].Column, Exp);
        }
        public static IAST LabelReductor(IAST[] asts,string newSymbol)
        {
           var node = asts[0];
           return new LabelAST(node.Symbol, node.Line, node.Column);
        }
        public static IAST GotoReductor(IAST[] asts,string newSymbol)
        {
            var label = asts[2];
            var con = asts[5];
            return new GoToAST(label, con, asts[6].Line, asts[6].Column);
        }
        public static IAST FunctionCallReductor(IAST[] asts,string newSymbol)
        {
            var idNode = (AtomicAST)asts[0];
            var args = (StmtListAST)asts[1];
            var arg = args.statements;
            return new FuncCallAST(idNode.Symbol,idNode.Line, idNode.Column,arg.ToArray());
        }
    }
}
