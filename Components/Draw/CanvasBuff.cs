namespace WallyInterpreter.Components.Draw
{
    public enum Colors
    {
        Transparent = 0,
        Black = 1,
        Blue = 2,
        Red = 3,
        Yellow = 4,
        Green = 5,
        Orange = 6,
        Purple = 7,
    }
    public static class CanvasBuff
    {
        public static event Action? OnChanged;

        public static int Rows { get; private set; } = 20;
        public static int Cols { get; private set; } = 20;
        public static Colors[,] Matrix { get; private set; }
            = new Colors[Rows, Cols];

        public static void SetCell(int x, int y, Colors c)
        {
            if (x < 0 || x >= Cols || y < 0 || y >= Rows) return;
            Matrix[y, x] = c;
            OnChanged?.Invoke();
        }

        public static void Clear()
        {
            for (var y = 0; y < Rows; y++)
                for (var x = 0; x < Cols; x++)
                    Matrix[y, x] = Colors.Transparent;
            OnChanged?.Invoke();
        }
        public static void Resize(int n)
        {
            Rows = n;
            Cols = n;
            Clear();
        }
    }
}
