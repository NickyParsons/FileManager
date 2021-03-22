using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager.UI
{
    class Figure
    {
        public char leftSymbol = '║';
        public char rightSymbol = '║';
        public char topSymbol = '═';
        public char bottomSymbol = '═';
        public char topLeftCornerSymbol = '╔';
        public char topRightCornerSymbol = '╗';
        public char bottomLeftCornerSymbol = '╚';
        public char bottomRightCornerSymbol = '╝';
        public List<Point> pList;
        public void Draw()
        {
            foreach (Point point in pList)
            {
                point.Draw();
            }
        }
    }
}
