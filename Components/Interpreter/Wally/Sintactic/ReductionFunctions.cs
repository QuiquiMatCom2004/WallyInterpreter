using WallyInterpreter.Components.Interpreter.Errors;
using WallyInterpreter.Components.Interpreter.Semantic;
using WallyInterpreter.Components.Interpreter.Wally.Lexical;

namespace WallyInterpreter.Components.Interpreter.Wally.Sintactic
{
    public static class ReductionFunctions
    {
        public static IAST BinaryOperator(IAST[]asts,string newSymbol)
        {
            var op = asts[1];
            var builder = new ASTBuilder();
            var bin = new BinaryAST(newSymbol, op.Line, op.Column, builder.BinaryOperators[op.Symbol]);
            bin.Left = asts[0];
            bin.Right = asts[2];
            return bin;
        }
        public static IAST UnaryOperatorReduction(IAST[] asts,string newSymbol)
        {
            var builder = new ASTBuilder();
            return new UnaryAST(newSymbol, asts[0].Line, asts[0].Column, builder.UnaryOperators[asts[0].Symbol], asts[1]);
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

            return new AssignationAST(newSymbol, asts[2].Line, asts[2].Column, VarName, Exp);
        }
        public static IAST LabelReductor(IAST[] asts,string newSymbol)
        {
           var node = asts[0];
           return new LabelAST(newSymbol, node.Line, node.Column, node);
        }
        public static IAST GotoReductor(IAST[] asts,string newSymbol)
        {
            var label = asts[2];
            var con = asts[5];
            return new GoToAST(newSymbol,label, con, asts[6].Line, asts[6].Column);
        }
        public static IAST FunctionCallReductor(IAST[] asts,string newSymbol)
        {
            var idNode = asts[0];
            var args = asts[2];
            if (args.Symbol == ")")
                return new FuncCallAST(newSymbol, idNode.Line, idNode.Column, idNode, Array.Empty<IAST>());
            var arg =((StmtListAST)args).statements;
            return new FuncCallAST(newSymbol,idNode.Line, idNode.Column, idNode, arg.ToArray());
        }
        public static IAST LeftBinaryOperator(IAST[] asts, string newSymbol)
        {
            var left = asts[0];
            var prime = asts[1];
            if (prime == null)
                return left;
            if (prime is BinaryAST bin)
            {
                bin.Left = left;
                return bin;
            }
            return left;
        }

        public static IAST RightBinaryOperatorReduction(IAST[] asts, string newSymbol)
        {
            if (asts.Length == 1 && asts[0] == null)
                return null; // epsilon

            var builder = new ASTBuilder();
            var op = asts[0].Symbol;
            var right = asts[1];
            var tail = asts.Length > 2 ? asts[2] : null;

            if (tail == null)
            {
                var bin = new BinaryAST(newSymbol, right.Line, right.Column, builder.BinaryOperators[op]);
                bin.Right = right;
                return bin;
            }
            var bina = new BinaryAST(newSymbol, right.Line, right.Column, builder.BinaryOperators[op]);
            bina.Right = tail;
            return bina;
        }
        public static IAST ArgsListReduction(IAST[] asts,string newSymbol)
        {
            var exp = asts[0];
            var argsprime = (asts.Length > 1) ? asts[1] : null;
            if (argsprime == null)
                return exp;
            if(argsprime is StmtListAST)
                return new StmtListAST(newSymbol,new[] { exp }.Concat(((StmtListAST)argsprime).statements).ToArray(), exp.Line, exp.Column);
            else if (argsprime is IAST ast)
                return new StmtListAST(newSymbol,new[] { exp, ast }, exp.Line, exp.Column);
            else
                return null;
        }
        public static IAST ArgListPrimeReduction(IAST[] asts, string newSymbols)
        {
            if (asts.Length == 0 || asts[0] == null)
                return null; // epsilon
            var args = new List<IAST>();
            foreach (var ast in asts)
            {
                if (ast != null)
                    args.Add(ast);
            }
            if (args.Count == 1)
                return args[0];
            else
                return new StmtListAST(newSymbols,args.ToArray(), asts[0].Line, asts[0].Column);
        }
        public static IAST StmtListReduction(IAST[] asts,string newSymbol)
        {
            if (asts.Length == 0 || asts[0] == null)
                return null; // epsilon
            var stmts = new List<IAST>() { asts[1] };
            ;
            return new StmtListAST(newSymbol, (((StmtListAST)asts[0]).statements).Concat(stmts).ToArray(), asts[0].Line, asts[0].Column);
        }

        public static IAST ArgsReductor(IAST[] asts,string newSymbol)
        {
            var args = (StmtListAST)asts[0];
            var expr = asts[2];
            args.statements.Add(expr);

            return new StmtListAST(newSymbol, args.statements.ToArray(),args.Line,args.Column);
        }
        public static IAST VariableReductor(IAST[] asts, string newSymbol)
        {
            var node = asts[0];
            return new VariableAST(newSymbol, node.Line, node.Column, node);
        }
    }
}
