namespace WallyInterpreter.Components.Interpreter.Granmar
{
    public interface IGranmar
    {
        IGranmarSymbol[] Terminals();
        IGranmarSymbol[] NonTerminals();
        IGranmarSymbol StartSymbol();
        void AddProduction(IGranmarSymbol symbol, IGranmarSymbol[] symbols);
        IGranmarSymbol[][] GetProduction(IGranmarSymbol symbol);
        IGranmarSymbol[] First(IGranmarSymbol[] symbol);
        IGranmarSymbol[] Follow(IGranmarSymbol symbol);
        void MakeFirstAndFollow(IGranmarSymbol symbol);
    }
}
