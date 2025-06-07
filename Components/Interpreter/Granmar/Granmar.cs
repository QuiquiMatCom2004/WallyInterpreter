using System.Security.Cryptography.X509Certificates;

namespace WallyInterpreter.Components.Interpreter.Granmar
{
    public class Granmar : IGranmar
    {
        private List<IGranmarSymbol> _terminals;
        private List<IGranmarSymbol> _nonTerminals;
        private Dictionary<string, List<List<IGranmarSymbol>>> _productions;
        private Dictionary<string, List<IGranmarSymbol>> _first;
        private Dictionary<string, List<IGranmarSymbol>> _follow;
        public Granmar(IGranmarSymbol start)
        {
            _terminals = new List<IGranmarSymbol>();
            _nonTerminals = new List<IGranmarSymbol>() {start};
            _first = new Dictionary<string, List<IGranmarSymbol>>();
            _follow = new Dictionary<string, List<IGranmarSymbol>>();
            _productions = new Dictionary<string, List<List<IGranmarSymbol>>>();
        }

        public void AddProduction(IGranmarSymbol symbol, IGranmarSymbol[] symbols)
        {
            if(symbol.Type() == GranmarSymbolType.Terminal)
            {
                throw new Exception("Terminals dont have productions");
            }
            if (!_productions.ContainsKey(symbol.Symbol()))
            {
                _productions[symbol.Symbol()] = new List<List<IGranmarSymbol>>();
            }
            if (_productions[symbol.Symbol()].Contains(symbols.ToList()))
            {
                throw new Exception("Already exist this production");
            }
            if (!_nonTerminals.Contains(symbol))
            {
                _nonTerminals.Add(symbol);
            }
            foreach(var s in symbols)
            {
                switch(s.Type())
                {
                    case GranmarSymbolType.Terminal:
                        if(!_terminals.Contains(s))
                            _terminals.Add(s);
                        break;
                    case GranmarSymbolType.NonTerminal:
                        if(!_nonTerminals.Contains(s))
                            _nonTerminals.Add(s);
                        break;
                    default:
                        continue;
                }
            }
            _productions[symbol.Symbol()].Add(symbols.ToList());
        }

        public IGranmarSymbol[] First(IGranmarSymbol[] symbol)
        {
            bool epsilon = symbol.Length > 0;
            int pos = 0;
            var result = new List<IGranmarSymbol>();
            while (pos < symbol.Length && epsilon) {
                foreach (var s in _first[symbol[pos].Symbol()]) { 
                    if(result.Contains(s) && !s.Epsilon())
                        result.Add(s);
                }
                epsilon = DeriveInEpsilon(symbol[pos]);
                pos++;
            }
            if (epsilon) {
                result.Add(_terminals.First(s => s.Epsilon()));
            }
            return result.ToArray();
        }

        public IGranmarSymbol[] Follow(IGranmarSymbol symbol)
        {
            return _follow[symbol.Symbol()].ToArray();
        }

        public IGranmarSymbol[][] GetProduction(IGranmarSymbol symbol)
        {
            if (symbol.Type() == GranmarSymbolType.Terminal || !_productions.ContainsKey(symbol.Symbol())|| _productions[symbol.Symbol()].ToArray() == null)
                return new IGranmarSymbol[][] { };
            List<IGranmarSymbol[]> result = new List<IGranmarSymbol[]>();
            foreach(var i in _productions[symbol.Symbol()].ToArray())
            {
                result.Add(i.ToArray());
            }
            return result.ToArray();
        }

        public void MakeFirstAndFollow(IGranmarSymbol endmarker)
        {
            MakeFirst();
            MakeFollow(endmarker);
        }

        public IGranmarSymbol[] NonTerminals()
        {
            return new List<IGranmarSymbol>(_nonTerminals).ToArray();
        }

        public IGranmarSymbol StartSymbol()
        {
            return new GranmarSymbol(_nonTerminals[0].Symbol(), _nonTerminals[0].Epsilon(), _nonTerminals[0].Type()) ;
        }

        public IGranmarSymbol[] Terminals()
        {
            return new List<IGranmarSymbol>(_terminals).ToArray();
        }

        private bool DeriveInEpsilon(IGranmarSymbol symbol)
        {
            foreach (var prod in GetProduction(symbol)) {
                if (prod.Length == 1 && prod[0].Epsilon()) return true; 
            }
            return false;
        }
        private void MakeFirst()
        {
            InitFirstSets();
            bool change;
            do
            {
                change = false;
                foreach(var symbol in _nonTerminals)
                {
                    foreach(var prod in _productions[symbol.Symbol()])
                    {
                        bool epsilon = true;
                        int pos = 0;
                        while(epsilon && pos < prod.Count())
                        {
                            foreach(var sym in _first[prod[pos].Symbol()])
                            {
                                if (_first[symbol.Symbol()].Contains(sym) && !sym.Epsilon())
                                {
                                    _first[symbol.Symbol()].Add(sym);
                                    change = true;
                                }
                            }
                            epsilon = DeriveInEpsilon(prod[pos]);
                            pos++;
                        }
                        if(epsilon)
                        {
                            _first[symbol.Symbol()].Add(_terminals.First(s => s.Epsilon()));
                            change = true;
                        }
                    }
                    if (change)
                        break;
                }
            }
            while (change);
        }
        private void MakeFollow(IGranmarSymbol endmarker)
        {
            InitFollowSets(endmarker);
            bool change;
            do
            {
                change = false;
                foreach ( var  symbol in _nonTerminals)
                {
                    if (MakeFollowFor(symbol))
                        change = true;
                }
            }
            while (change);
        }
        private void InitFirstSets()
        {
            List<IGranmarSymbol> symbols = new List<IGranmarSymbol>();
            foreach(var terminal in _terminals)
            {
                symbols.Add(terminal);
            }
            foreach(var nonterminals in _nonTerminals)
            {
                symbols.Add(nonterminals);
            }
            foreach(var symbol in symbols)
            {
                if(symbol.Type() == GranmarSymbolType.Terminal)
                {
                    _first[symbol.Symbol()] = new List<IGranmarSymbol> { symbol };
                    continue;
                }
                if(symbol.Type() == GranmarSymbolType.NonTerminal && DeriveInEpsilon(symbol))
                {
                    _first[symbol.Symbol()].Add(_terminals.First(s => s.Epsilon()));
                    continue;
                }
                _first[symbol.Symbol()] = new List<IGranmarSymbol>();
            }
        }
        private void InitFollowSets(IGranmarSymbol endmarker)
        {
            foreach(var symbol in _nonTerminals)
            {
                _follow[symbol.Symbol()] = new List<IGranmarSymbol>();
            }
            _follow[StartSymbol().Symbol()].Add(endmarker);
        }
        private bool MakeFollowFor(IGranmarSymbol endmarker)
        {
            bool result = false;
            foreach(var nonTerminal in _nonTerminals)
            {
                if (!_productions.ContainsKey(nonTerminal.Symbol())) continue;
                foreach (var prod in _productions[nonTerminal.Symbol()]) {
                    int index = prod.FindIndex(s => s.Symbol() == endmarker.Symbol());
                    if (index != -1) {
                        List<IGranmarSymbol> first = First(prod.Skip(index+1).ToArray()).ToList();
                        bool epsilon = first.Any(s => s.Epsilon());
                        if (prod.Count()-1 == index ||epsilon) {
                            foreach (var syms in _follow[nonTerminal.Symbol()]) {
                                if (!_follow[endmarker.Symbol()].Any(s => s.Symbol() == syms.Symbol())&& !syms.Epsilon())
                                {
                                    _follow[endmarker.Symbol()].Add(syms);
                                    result = true;
                                }
                            }
                        }
                        foreach(var syms in first)
                        {
                            if (!_follow[endmarker.Symbol()].Any(s => s.Symbol() == syms.Symbol()) && !syms.Epsilon())
                            {
                                _follow[endmarker.Symbol()].Add(syms);
                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }
        public override string ToString()
        {
            var A = () =>
            {
                string result = "";
                foreach(var t in Terminals())
                    result += t.Symbol() + " ";
                return result;
            };
            var B = () =>
            {
                string result = "";
                foreach (var nt in NonTerminals())
                    result += nt.Symbol() + " ";
                return result;
            };
            var C = () =>
            {
                string result = "";
                foreach (var f in _first.Keys) { 
                    result += $"for this {f} symbol the first is {{";
                    foreach(var s in _first[f])
                    {
                        result += s.Symbol() + " ";
                    }
                    result += "}";
                }
                return result;
            };
            var D = () =>
            {
                string result = "";
                foreach (var f in _follow.Keys)
                {
                    result += $"for this {f} symbol the follow is {{";
                    foreach (var s in _follow[f])
                    {
                        result += s.Symbol() + " ";
                    }
                    result += "}";
                }
                return result;
            };
            var E = () =>
            {
                string result = "";
                foreach (var f in _productions.Keys)
                {
                    result += $"for this {f} symbol the production is {{";
                    foreach (var s in _productions[f])
                    {
                        result += " ";
                        foreach(var p in s)
                        {
                            result += p.Symbol() +" | ";
                        }
                    }
                    result += "}";
                }
                return result;
            };
            return $" simbolo inicial {StartSymbol().Symbol()}, Terminales:{ A()}, No Terminales:{ B()}, First: { C()} Follow: {D()} Productions: {E()}";
        }
    }
}
