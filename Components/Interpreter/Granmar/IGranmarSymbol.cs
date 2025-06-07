namespace WallyInterpreter.Components.Interpreter.Granmar
{
    public interface IGranmarSymbol
    {         
        /// <summary>
        /// Return the symbol the represent
        /// </summary>
        /// <returns></returns>
        string Symbol();
        /// <summary>
        /// Say if this symbol its epsilon or not
        /// </summary>
        /// <returns> True its epsilon</returns>
        bool Epsilon();
        GranmarSymbolType Type();

    }
}
