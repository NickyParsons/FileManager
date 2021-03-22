using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager.UI
{
    class Point
    {
        char symbol;
        int x;
        int y;
        public Point(int x, int y)
        {
            symbol = '*';
            this.x = x;
            this.y = y;
        }
        public Point(int x, int y, char sym)
        {
            symbol = sym;
            this.x = x;
            this.y = y;
        }
        public void Draw()
        {
            Console.SetCursorPosition(this.x, this.y);
            Console.Write(this.symbol);
        }
    }
}
