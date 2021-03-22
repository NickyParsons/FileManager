using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager.UI
{
    class Square : Figure
    {
        public int posBorderLeft;
        public int posBorderRight;
        public int posBorderTop;
        public int posBorderBottom;
        int areaX;
        int areaY;
        int rowLength;
        int rows;
        int currentRow;

        public Square(int left, int top, int width, int height)
        {
            this.pList = new List<Point>();
            this.posBorderLeft = left;
            this.posBorderTop = top;
            this.posBorderRight = left + width - 1;
            this.posBorderBottom = top + height - 1;
            this.areaX = posBorderLeft + 1;
            this.areaY = posBorderTop + 1;
            this.rows = height - 2;
            this.rowLength = width - 2;
            this.currentRow = areaY;
        }
        void MakeBorderLeft()
        {
            pList.Add(new Point(posBorderLeft, posBorderTop, topLeftCornerSymbol));
            pList.Add(new Point(posBorderLeft, posBorderBottom, bottomLeftCornerSymbol));
            for (int i = posBorderTop + 1; i < posBorderBottom; i++)
            {
                pList.Add(new Point(posBorderLeft, i, leftSymbol));
            }
        }
        void MakeBorderRight()
        {
            pList.Add(new Point(posBorderRight, posBorderTop, topRightCornerSymbol));
            pList.Add(new Point(posBorderRight, posBorderBottom, bottomRightCornerSymbol));
            for (int i = posBorderTop + 1; i < posBorderBottom; i++)
            {
                pList.Add(new Point(posBorderRight, i, rightSymbol));
            }
        }
        void MakeBorderTop()
        {
            pList.Add(new Point(posBorderLeft, posBorderTop, topLeftCornerSymbol));
            pList.Add(new Point(posBorderRight, posBorderTop, topRightCornerSymbol));
            for (int i = posBorderLeft + 1; i < posBorderRight; i++)
            {
                pList.Add(new Point(i, posBorderTop, topSymbol));
            }
        }
        void MakeBorderBottom()
        {
            pList.Add(new Point(posBorderLeft, posBorderBottom, bottomLeftCornerSymbol));
            pList.Add(new Point(posBorderRight, posBorderBottom, bottomRightCornerSymbol));
            for (int i = posBorderLeft + 1; i < posBorderRight; i++)
            {
                pList.Add(new Point(i, posBorderBottom, bottomSymbol));
            }
        }
        public void MakeAllBorders()
        {
            this.MakeBorderLeft();
            this.MakeBorderRight();
            this.MakeBorderTop();
            this.MakeBorderBottom();
        }
        public string Input(string text)
        {
            Console.SetCursorPosition(areaX, areaY);
            string outString;
            Console.Write(text);
            outString = Console.ReadLine();
            this.Clear();
            return outString;
        }
        public void PrintLine(string text)
        {
            Console.SetCursorPosition(areaX, currentRow);
            Console.Write(text);
            currentRow++;
        }
        public void DisplayLine(string text)
        {
            this.Clear();
            PrintLine(text);
        }
        public void DisplayAll(string[] text)
        {
            this.Clear();
            foreach (string line in text)
            {
                PrintLine(line);
            }
        }
        public void Clear()
        {
            this.currentRow = this.areaY;
            for (int i = 0; i < rows; i++)
            {
                this.PrintLine(new string(' ', rowLength));
            }
            this.currentRow = this.areaY;
        }
    }
}
