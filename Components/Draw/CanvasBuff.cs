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
        White = 8
    }
    public class CanvasBuff
    {
        public event Action? OnChanged;

        public int Rows { get; private set; } = 20;
        public int Cols { get; private set; } = 20;
        public Colors[,] Matrix { get; private set; }
        public CanvasBuff(int Size)
        {
            Rows = Size;
            Cols = Size;
            Matrix = new Colors[Size,Size];
        }

        public void SetCell(int x, int y, Colors c)
        {
            if (x < 0 || x >= Cols || y < 0 || y >= Rows) return;
            Matrix[y, x] = c;
            OnChanged?.Invoke();
        }

        public void Clear()
        {
            for (var y = 0; y < Rows; y++)
                for (var x = 0; x < Cols; x++)
                    Matrix[y, x] = Colors.White;
            OnChanged?.Invoke();
        }
        public void Resize(int n)
        {
            Rows = n;
            Cols = n;
            Clear();
        }
    }
}
