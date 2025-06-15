using WallyInterpreter.Components.Interpreter.Granmar;

namespace WallyInterpreter.Components.Interpreter.Parser
{
    public class ParserTools
    {
        public IItemCollectionLR0 GetLR0Collection(IGranmarSymbol head, IGranmarSymbol[] production)
        {
            List<IItemLR0> items = new List<IItemLR0>();
            for (int i = 0; i < production.Length; i++)
            {
                IGranmarSymbol[] left = production.Take(i).ToArray();
                IGranmarSymbol[]right = production.Skip(i).ToArray();
                items.Add(new ItemLR0(head, left, right));
            }
            return new ItemCollectionLR0( items.ToArray());
        }
        public IItemCollectionLR0 ItemLR0Clousure(IItemCollectionLR0 items,IGranmar granmar)
        {
            var initial = items.ToArray();
            var cola = new Queue<IItemLR0>(initial);
            var visited = new Dictionary<string, IItemLR0>(StringComparer.Ordinal);
            while(cola.Count > 0)
            {
                var current = cola.Dequeue();
                if (visited.ContainsKey(current.ID))
                    continue;
                visited[current.ID] = current;
                var afterDot = current.RightPoint;
                if(afterDot != null && afterDot.Length > 0)
                {
                    var sym = afterDot[0];
                    var prods = granmar.GetProduction(sym);
                    foreach (var prod in prods) 
                    {
                        var newItem = new ItemLR0(sym,Array.Empty<IGranmarSymbol>(),prod);
                        if (!visited.ContainsKey(newItem.ID))
                            cola.Enqueue(newItem);
                    }
                }
            }
            return new ItemCollectionLR0(visited.Values.ToArray());
        }
        public IItemCollectionLR0 GOTO(IItemCollectionLR0 I,IGranmarSymbol symbol, IGranmar granmar)
        {
            List<IItemLR0> items = new List<IItemLR0>();
            foreach (var item in I)
            {
                if(item.RightPoint.Length > 0 && item.RightPoint[0].Symbol() == symbol.Symbol())
                {
                    var newLeft = item.LeftPoint.Concat(new IGranmarSymbol[] { item.RightPoint[0] }).ToArray();
                    var newRight = item.RightPoint.Skip(1).ToArray();

                    var newItem = new ItemLR0(item.Head,newLeft,newRight);
                    items.Add(newItem);
                }
            }
            if (items.Count > 0) {
                ItemCollectionLR0 tempCollection = new ItemCollectionLR0(items.ToArray());
                return ItemLR0Clousure(tempCollection, granmar);
            }
            return null;
        }
        public IItemCollectionLR0[] GetCanonicalLR0Collection(IGranmar g)
        {
            IGranmarSymbol oldStart = g.GetProduction(g.StartSymbol())[0][0];
            IItemLR0 startItem = new ItemLR0(g.StartSymbol(), new IGranmarSymbol[0], new IGranmarSymbol[] { oldStart });

           
            IItemCollectionLR0 startClosure = ItemLR0Clousure(new ItemCollectionLR0(new IItemLR0[] { startItem }), g);

            var all = new List<IItemCollectionLR0>() { startClosure};
            var pending = new Queue<IItemCollectionLR0>();
            pending.Enqueue(startClosure);

            var symbols = g.NonTerminals().Where(nt => nt.Symbol() != g.StartSymbol().Symbol()).Cast<IGranmarSymbol>().Concat(g.Terminals()).ToArray();
            while(pending.Count > 0)
            {
                var c = pending.Dequeue();
                foreach(var sym in symbols)
                {
                    var gotoc = GOTO(c, sym, g);
                    if (gotoc == null)
                        continue;
                    if(!all.Any(x => x.ID == gotoc.ID))
                    {
                        all.Add(gotoc);
                        pending.Enqueue(gotoc);
                    }
                }
            }
            return all.ToArray();
        }
    }
}
