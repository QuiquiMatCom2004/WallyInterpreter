using BlazorMonaco.Languages;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System.Drawing;
using System.Net.Http.Headers;
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
        public GlobalFunctions() {
            wallyStruct.X = -1;
            wallyStruct.Y = -1;
        }
        Colors actualColor = Colors.Transparent;
        int actualSizeBrush = 1;
        private void Draw(int x, int y,CanvasBuff canvas)
        {
            if (actualColor == Colors.Transparent)
                return;
            for (int i = x - (actualSizeBrush - 1) / 2; i < x + actualSizeBrush; i++) 
            {
                for (int j = y - (actualSizeBrush - 1) / 2; j < y + actualSizeBrush; j++) 
                {
                    canvas.SetCell(j,i , actualColor);
                }
            }
        }
        public void Spawn(List<object> param)
        {
            if (param.Count() != 3) throw new ArgumentException();
            int x = Convert.ToInt32(param[0]);
            int y = Convert.ToInt32(param[1]);
            CanvasBuff canvas = (CanvasBuff)param[2];
            if (x < 0 || canvas.Rows < x || y < 0 || canvas.Cols < y) {
                throw new IndexOutOfRangeException();
            }
            wallyStruct.X = x; wallyStruct.Y = y; 
        }
        public void Color(List<object> param)
        {
            if (param.Count() != 2) throw new ArgumentException();
            string color = Convert.ToString(param[0]);
            color = RemoveDoubleQuote(color);
            actualColor = colorTranslation[color];
        }
        private string RemoveDoubleQuote(string s)
        {
            return s.Substring(1, s.Length - 2);
        }
        public void Size(List<object> param)
        {
            if(param.Count() != 2)throw new ArgumentException();
            int x = Convert.ToInt32(param[0]);
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
        public void DrawLine(List<object> param) 
        {
            if(param.Count() != 4) throw new ArgumentException();
            int x = Convert.ToInt32( param[0]);
            int y = Convert.ToInt32(param[1]);
            int dist = Convert.ToInt32(param[2]);
            CanvasBuff canvas = (CanvasBuff)param[3];
            if (Math.Abs(x) > 1 || Math.Abs(y) > 1) return;
            if (x == 1 && canvas.Rows - wallyStruct.X < dist ||x ==-1 &&wallyStruct.X < dist || y == 1 && canvas.Cols - wallyStruct.Y < dist || y ==-1 && wallyStruct.Y < dist)
                throw new IndexOutOfRangeException();
            while (dist > 0) {
                Draw(wallyStruct.X, wallyStruct.Y,canvas);
                wallyStruct.X += x;
                wallyStruct.Y += y;
                dist--;
            }
        }
        public void DrawCircle(List<object> param)
        {
            if(param.Count() != 4) throw new ArgumentException();
            int x = Convert.ToInt32(param[0]);
            int y = Convert.ToInt32(param[1]);
            int radius = Convert.ToInt32(param[2]);
            CanvasBuff canvas = (CanvasBuff)param[3];
            if (wallyStruct.X + x > canvas.Rows && x > 0 || wallyStruct.Y + y > canvas.Cols && y>0|| wallyStruct.Y + y < 0  && y < 0|| wallyStruct.X + x < 0 && x < 0)
                throw new IndexOutOfRangeException();
            wallyStruct.X += x;
            wallyStruct.Y += y;
            var cx = wallyStruct.X;
            var cy = wallyStruct.Y;
            var d = 3 - 2 * radius;
            x = 0;
            y = radius;
            while(y >= x)
            {
                Plot8(canvas, cx, cy, x,y);
                if(d < 0)
                {
                    d += 4 * x + 6;
                }
                else
                {
                    d += 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }
            
        }
        private void Plot8(CanvasBuff canvas, int cx,int cy, int x,int y)
        {
            if(cx+x >=0 &&cx+x < canvas.Rows && cy + y >=0 && cy + y < canvas.Cols)
                Draw(cx+x, cy+y, canvas);
            if (cx - x >= 0 && cx - x < canvas.Rows && cy + y >= 0 && cy + y < canvas.Cols)
                Draw(cx - x, cy + y, canvas);
            if (cx + x >= 0 && cx + x < canvas.Rows && cy - y >= 0 && cy - y < canvas.Cols)
                Draw(cx + x, cy - y, canvas);
            if (cx - x >= 0 && cx - x < canvas.Rows && cy - y >= 0 && cy - y < canvas.Cols)
                Draw(cx - x, cy - y, canvas);
            if (cx + y >= 0 && cx + y < canvas.Rows && cy + x >= 0 && cy + x < canvas.Cols)
                Draw(cx + y, cy + x, canvas);
            if (cx - y >= 0 && cx - y < canvas.Rows && cy + x >= 0 && cy + x < canvas.Cols)
                Draw(cx - y, cy + x, canvas);
            if (cx + y >= 0 && cx + y < canvas.Rows && cy - x >= 0 && cy - x < canvas.Cols)
                Draw(cx + y, cy - x, canvas);
            if (cx - y >= 0 && cx - y < canvas.Rows && cy - x >= 0 && cy - x < canvas.Cols)
                Draw(cx - y, cy - x, canvas);
        }
        public void DrawRectangle(List<object> param)
        {
            if (param.Count() != 6) throw new ArgumentException();
            int dirX = Convert.ToInt32(param[0]);
            int dirY = Convert.ToInt32(param[1]);
            int dist = Convert.ToInt32(param[2]);
            int with = Convert.ToInt32(param[3]);
            int heigth = Convert.ToInt32(param[4]);
            CanvasBuff canvas = (CanvasBuff)param[5];
            if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1) return;
            while (dist > 0) {
                if (wallyStruct.X + dirX > canvas.Rows || wallyStruct.Y + dirY > canvas.Cols || wallyStruct.Y + dirY < 0 || wallyStruct.X + dirX < 0)
                    throw new IndexOutOfRangeException();
                wallyStruct.X += dirX;
                wallyStruct.Y += dirY;
                dist--;
            }
            var x = wallyStruct.X;
            var y = wallyStruct.Y;
            wallyStruct.X -= heigth / 2;
            wallyStruct.Y -= with / 2;
            DrawLine(new List<object>(){1, 0, heigth, canvas});
            
            DrawLine(new List<object>() { 0, 1, with, canvas });

            DrawLine(new List<object>() { -1, 0, heigth, canvas });

            DrawLine(new List<object>() { 0, -1, with, canvas });

            wallyStruct.X = x;
            wallyStruct.Y = y;
        }
        public void Fill(List<object> param)
        {
            if(param.Count() != 1)throw new ArgumentException();
            CanvasBuff canvas = (CanvasBuff)param[0];
            Queue<(int,int)> cola = new Queue<(int, int)> ();
            cola.Enqueue((wallyStruct.X, wallyStruct.Y));
            Colors color = canvas.Matrix[wallyStruct.Y,wallyStruct.X];
            while (cola.Count > 0) {
                var pos = cola.Dequeue ();
                CheckAdyacentsColors(pos.Item2, pos.Item1, cola, color,canvas);
                canvas.SetCell(pos.Item1, pos.Item2, actualColor);
            }
        }
        private void CheckAdyacentsColors(int x, int y, Queue<(int,int)> cola, Colors color,CanvasBuff canvas)
        { 
            if (IsInMatrix(x - 1, y, canvas) && !cola.Any(item => item.Item1 == x-1 && item.Item2 == y) && canvas.Matrix[x-1,y] == color)
            {
                cola.Enqueue((x-1, y));
            }
            if (IsInMatrix(x + 1, y, canvas) && !cola.Any(item => item.Item1 == x + 1 && item.Item2 == y) && canvas.Matrix[x +1, y] == color)
            {
                cola.Enqueue((x+1, y));
            }
           /* if (IsInMatrix(x - 1, y-1, canvas) && !cola.Any(item => item.Item1 == x - 1 && item.Item2 == y-1) && canvas.Matrix[x - 1, y-1] == color)
            {
                cola.Enqueue((x-1, y-1));
            }*/
           /* if (IsInMatrix(x-1,y+1,canvas) && !cola.Any(item => item.Item1 == x - 1 && item.Item2 == y+1) && canvas.Matrix[x - 1, y + 1] == color)
            {
                cola.Enqueue((x - 1, y+1));
            }*/
           /* if (IsInMatrix(x + 1,y-1,canvas) && !cola.Any(item => item.Item1 == x + 1 && item.Item2 == y-1) && canvas.Matrix[x +1, y -1] == color)
            {
                cola.Enqueue((x+1, y - 1));
            }
            if (IsInMatrix(x + 1,y + 1,canvas) && !cola.Any(item => item.Item1 == x + 1 && item.Item2 == y+1) && canvas.Matrix[x + 1, y + 1] == color)
            {
                cola.Enqueue((x+1, y+1));
            }*/
            if (IsInMatrix(x,y - 1,canvas) && !cola.Any(item => item.Item1 == x && item.Item2 == y-1) && canvas.Matrix[x, y -1] == color)
            {
                cola.Enqueue((x, y - 1));
            }
            if (IsInMatrix(x,y + 1,canvas) && !cola.Any(item => item.Item1 == x  && item.Item2 == y+1) &&  canvas.Matrix[x , y+1] == color)
            {
                cola.Enqueue((x, y+1));
            }
        }
        private bool IsInMatrix(int x, int y, CanvasBuff canvas)
        {
            if (x < 0 || x >= canvas.Rows || y < 0 || y >= canvas.Cols)
            {
                return false;
            }
            return true;
        }
        public int GetActualX(List<object> param) 
        {
            if(param.Count() != 1) throw new ArgumentException();
            return wallyStruct.X;
        }
        public int GetActualY(List<object> param)
        {
            if (param.Count() != 1) throw new ArgumentException();
            return wallyStruct.Y;
        }
        public int GetCanvasSize(List<object> param)
        {
            if(param.Count() != 1) throw new ArgumentException();
            CanvasBuff canvas = (CanvasBuff)param[0];
            return canvas.Cols;
        }
        public int GetColorCount(List<object> param)
        {
            if (param.Count() != 6) throw new ArgumentException();
            string color = Convert.ToString(param[0]);
            color = RemoveDoubleQuote(color);
            int x1 = Convert.ToInt32(param[1]);
            int y1 = Convert.ToInt32(param[2]);
            int x2 = Convert.ToInt32(param[3]);
            int y2 = Convert.ToInt32(param[4]);
            CanvasBuff canvas = (CanvasBuff)param[5];
            Colors c = colorTranslation[color];
            int result = 0;
            if (x1 >= canvas.Rows || x2 >= canvas.Rows || x1 < 0 || x2 < 0 || y1 >= canvas.Cols || y2 >= canvas.Cols || y1 < 0 || y2 < 0)
                return 0;
            for (int i = x1; i <= x2; i++) {
                for (int j = y1; j <= y2; j++) { 
                    if(canvas.Matrix[j,i] == c) { result++; }
                }
            }
            return result;
        }
        public bool IsBrushColor(List<object> param)
        {
            if(param.Count() != 2) throw new ArgumentException();
            string color = Convert.ToString(param[0]);
            color = RemoveDoubleQuote(color);
            CanvasBuff canvas = (CanvasBuff)param[1];
            Colors c = colorTranslation[color];
            return c == actualColor;
        }
        public bool IsBrushSize(List<object> param)
        {
            if (param.Count() != 2) throw new ArgumentException();
            int Size = Convert.ToInt32(param[0]);
            return Size == actualSizeBrush;
        }
        public bool IsCanvasColor(List<object> param)
        {
            if(param.Count() != 4) throw new ArgumentException();
            string color = Convert.ToString(param[0]);
            color = RemoveDoubleQuote(color);
            int vertical = Convert.ToInt32(param[1]);
            int horizontal = Convert.ToInt32(param[2]);
            CanvasBuff canvas = (CanvasBuff)param[3];
            Colors c = colorTranslation[color];
            return canvas.Matrix[wallyStruct.Y + vertical,wallyStruct.X + horizontal] == c;
        }
    }
}
