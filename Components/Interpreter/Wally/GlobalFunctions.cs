using System.Net.Http.Headers;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using WallyInterpreter.Components.Draw;

namespace WallyInterpreter.Components.Interpreter.Wally
{
    struct WallyStruct
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class GlobalFunctions
    {
        WallyStruct wallyStruct;
        Dictionary<string,Colors> colorTranslation = new Dictionary<string, Colors>() {
            {"Black",Colors.Black },
            { "Transparent",Colors.Transparent},
            { "White",Colors.Transparent},
            {"Blue",Colors.Blue },
            { "Red",Colors.Red},
            { "Yellow",Colors.Yellow},
            { "Green", Colors.Green},
            { "Orange",Colors.Orange},
            { "Purple",Colors.Purple},
        };
        Colors actualColor = Colors.Transparent;
        int actualSizeBrush = 1;
        private void Draw(int x, int y)
        {
            for (int i = x - (actualSizeBrush - 1) / 2; i < x + actualSizeBrush; i++) 
            {
                for (int j = y - (actualSizeBrush - 1) / 2; j < y + actualSizeBrush; j++) 
                {
                    CanvasBuff.SetCell(j,i , actualColor);
                }
            }
        }
        public void Spawn(int x,int y)
        {
            if (x < 0 || CanvasBuff.Rows < x || y < 0 || CanvasBuff.Cols < y) {
                throw new IndexOutOfRangeException();
            }
            wallyStruct.X = x; wallyStruct.Y = y;
        }
        public void Color(string color)
        {
            actualColor = colorTranslation[color];
        }
        public void Size(int x)
        {
            if (x <= 0)
            {
                actualSizeBrush = 1;
            }
            else if (x % 2 == 0)
            {
                actualSizeBrush = x--;
            }
            else
            {
                actualSizeBrush = x;
            }
        }
        public void DrawLine(int x, int y, int dist) 
        {
            if (CanvasBuff.Rows - wallyStruct.X < dist || CanvasBuff.Cols - wallyStruct.Y < dist) {
                throw new IndexOutOfRangeException();
            }
            if (Math.Abs(x) > 1 || Math.Abs(y) > 1) return;
            while (dist > 0) {
                Draw(wallyStruct.X, wallyStruct.Y);
                wallyStruct.X += x;
                wallyStruct.Y += y;
                dist--;
            }
        }
        public void DrawCircle(int x, int y, int radius) 
        {
            if (wallyStruct.X + x > CanvasBuff.Rows || wallyStruct.Y + y > CanvasBuff.Cols || wallyStruct.Y + y < 0 || wallyStruct.X + x < 0)
                throw new IndexOutOfRangeException();
            wallyStruct.X += x;
            wallyStruct.Y += y;
            x = wallyStruct.X;
            y = wallyStruct.Y;
            if (x - radius < 0 || x + radius > CanvasBuff.Rows || y - radius < 0 || y + radius > CanvasBuff.Rows)
                throw new IndexOutOfRangeException();
            for(int i = y - radius; i < y + radius; i++)
            {
                int pto0 = (int)Math.Round( Math.Pow(Math.Pow(radius,2) + Math.Pow(y-radius,2),0.5));
                for(int j = x - radius; j < x + radius; j++)
                {
                    if(!(x < radius - pto0 || x > radius + pto0))
                    {
                        Draw(x, y);
                    }
                }
            }
        }
        public void DrawRectangle(int dirX, int dirY, int dist,int with, int heigth)
        {
            if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1) return;
            while (dist > 0) {
                if (wallyStruct.X + dirX > CanvasBuff.Rows || wallyStruct.Y + dirY > CanvasBuff.Cols || wallyStruct.Y + dirY < 0 || wallyStruct.X + dirX < 0)
                    throw new IndexOutOfRangeException( );
                wallyStruct.X += dirX;
                wallyStruct.Y += dirY;
                dist--;
            }
            var x = wallyStruct.X;
            var y = wallyStruct.Y;
            wallyStruct.X -= heigth / 2;
            wallyStruct.Y -= with / 2;
            DrawLine(1, 0, heigth);

            wallyStruct.X = x;
            wallyStruct.Y = y;
            wallyStruct.X += heigth / 2;
            wallyStruct.Y += with / 2;
            DrawLine(-1, 0, heigth);

            wallyStruct.X = x;
            wallyStruct.Y = y;
            wallyStruct.X += heigth / 2;
            wallyStruct.Y += with / 2;
            DrawLine(-1, 0, with);

            wallyStruct.X = x;
            wallyStruct.Y = y;
            wallyStruct.X -= heigth / 2;
            wallyStruct.Y -= with / 2;
            DrawLine(1, 0, with);

            wallyStruct.X = x;
            wallyStruct.Y = y;
        }
        public void Fill()
        {
            Queue<(int,int)> cola = new Queue<(int, int)> ();
            cola.Enqueue((wallyStruct.X, wallyStruct.Y));
            Colors color = CanvasBuff.Matrix[wallyStruct.Y,wallyStruct.X];
            while (cola.Count > 0) {
                var pos = cola.Dequeue ();
                CheckAdyacentsColors(pos.Item2, pos.Item1, cola, color);
                CanvasBuff.SetCell(pos.Item1, pos.Item2, actualColor);
            }
        }
        private void CheckAdyacentsColors(int x, int y, Queue<(int,int)> cola, Colors color)
        { 
            if (CanvasBuff.Matrix[x-1,y] == color)
            {
                cola.Enqueue((x, y));
            }
            if (CanvasBuff.Matrix[x +1, y] == color)
            {
                cola.Enqueue((x, y));
            }
            if (CanvasBuff.Matrix[x - 1, y-1] == color)
            {
                cola.Enqueue((x, y));
            }
            if (CanvasBuff.Matrix[x - 1, y + 1] == color)
            {
                cola.Enqueue((x, y));
            }
            if (CanvasBuff.Matrix[x +1, y -1] == color)
            {
                cola.Enqueue((x, y));
            }
            if (CanvasBuff.Matrix[x + 1, y + 1] == color)
            {
                cola.Enqueue((x, y));
            }
            if (CanvasBuff.Matrix[x, y -1] == color)
            {
                cola.Enqueue((x, y));
            }
            if (CanvasBuff.Matrix[x , y+1] == color)
            {
                cola.Enqueue((x, y));
            }
        }
        public int GetActualX() 
        {
            return wallyStruct.X;
        }
        public int GetActualY()
        {
            return wallyStruct.Y;
        }
        public int GetCanvasSize()
        {
            return CanvasBuff.Cols;
        }
        public int GetColorCount(string color,int x1,int y1,int x2, int y2)
        {
            Colors c = colorTranslation[color];
            int result = 0;
            for (int i = x1; i <= x2; i++) {
                for (int j = y1; j <= y2; j++) { 
                    if(CanvasBuff.Matrix[j,i] == c) { result++; }
                }
            }
            return result;
        }
        public bool IsBrushColor(string color)
        {
            Colors c = colorTranslation[color];
            return c == actualColor;
        }
        public bool IsBrushSize(int Size)
        {
            return Size == actualSizeBrush;
        }
        public bool IsCanvasColor(string color,int vertical,int  horizontal)
        {
            Colors c = colorTranslation[color];
            return CanvasBuff.Matrix[wallyStruct.Y + vertical,wallyStruct.X + horizontal] == c;
        }
    }
}
