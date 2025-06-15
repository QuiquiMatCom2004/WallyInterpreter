namespace WallyInterpreter.Components.Interpreter.Granmar
{
    /// <summary>
    /// This object contain the tools for build a granmar and the operations over the granmar
    /// </summary>
    public static class GranmarTools
    {
        public static IGranmar AddWordToGranmar(IGranmar granmar, string word)
        {
            IGranmarSymbol head = granmar.StartSymbol();
            IGranmarSymbol tail;
            for (int i = 0; i < word.Length; i++) { 
                tail = new GranmarSymbol(word + "_next_"+word[i]+i.ToString(),false,GranmarSymbolType.NonTerminal);
                if (i < word.Length - 1) {
                    granmar.AddProduction(head, new IGranmarSymbol[] { new GranmarSymbol(word[i].ToString(), false, GranmarSymbolType.Terminal), tail });
                }
                else
                {
                    granmar.AddProduction(head, new IGranmarSymbol[] { new GranmarSymbol(word[i].ToString(), false, GranmarSymbolType.Terminal)});
                }
                head = tail;
            }
            return granmar;
        }
        public static IGranmar GetWordsGrammar(string[] words)
        {
            IGranmarSymbol start = new GranmarSymbol("start_symbol",false,GranmarSymbolType.NonTerminal);
            IGranmar granmar = new Granmar(start);
            foreach (string word in words) { 
                granmar = AddWordToGranmar(granmar,word);
            }
            return granmar;
        }
        public static IGranmar Union(IGranmar[] granmars, string start_symbol_id) 
        {
            if (granmars.Length < 2) throw new Exception("Need at less 2 granmars");
            IGranmarSymbol start_symbol = new GranmarSymbol(start_symbol_id, false, GranmarSymbolType.NonTerminal);
            IGranmar g_result = new Granmar(start_symbol);
            foreach (var granmar in granmars) {
                g_result.AddProduction(start_symbol, new IGranmarSymbol[] { granmar.StartSymbol() });
                foreach (var nt in granmar.NonTerminals())
                {
                    foreach (var pro in granmar.GetProduction(nt))
                    {
                        g_result.AddProduction(nt, pro);
                    }
                }
            }
            return g_result;
        }
        public static IGranmar Augment(IGranmar granmar)
        {
            IGranmarSymbol startSymbol = new GranmarSymbol(granmar.StartSymbol().Symbol() + "_new_start", false, GranmarSymbolType.NonTerminal);
            IGranmar gresult = new Granmar(startSymbol);
            gresult.AddProduction(startSymbol, new IGranmarSymbol[] { granmar.StartSymbol() });
            foreach(var nt in granmar.NonTerminals())
            {
                foreach(var pro in granmar.GetProduction(nt))
                {
                    gresult.AddProduction(nt, pro);
                }
            }
            return gresult;
        }
    }
}
