using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Wally.Lexical;

namespace WallyInterpreter.Components.Interpreter.Wally.Sintactic
{
    public static class ReductionFunctions
    {
        public static IAST BinaryOperatorReduction(IAST[] asts,string newSymbol)
        {
            if (asts[1] is GarbageAST g) return g;
            var binary = (BinaryAST) asts[1];
            binary.Left = asts[0];
            binary.Right = asts[2];
            binary.UpdateSymbol(newSymbol);
            return binary;
        }
        public static IAST UnaryOperatorReduction(IAST[] asts,string newSymbol)
        {
            if (asts[0] is GarbageAST g) return g;
            var op = (UnaryAST)asts[0];
            op.target = asts[1];
            op.UpdateSymbol(newSymbol);
            return op;
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
            var node = (AtomicAST)asts[0];
            return new LabelAST(node.Eval(new Context(),new ErrorColector()).ToString(), node.Line, node.Column);
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
            List<IAST> args = new List<IAST>();
            int pos = 2;
            while (asts[pos].Symbol != ")")
            {
                if (asts[pos].Symbol == ",")
                {
                    pos++;
                    continue;
                }
                args.Add(asts[pos]);
                pos++;
            }
            return new FuncCallAST(idNode.Eval(new Context(), new ErrorColector()).ToString(), args.Last().Line, args.Last().Column + 1,args.ToArray());
        }
    }
}
