using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public class MoveWindowFunction
    {
        public static void ImageSegmentation(Rectangle rectangle, int poolWidth, int poolHeight, Action<Rectangle> callback)
        {
            double columnCount = rectangle.Width / poolWidth;
            double rowCount = rectangle.Height / poolHeight;
            int columnCurrentCount = 0;
            int rowCurrentCount = 0;
            int columnValue = rectangle.X;
            int rowValue = rectangle.Y;
            Rectangle[] rectangles;
            rectangles = new Rectangle[(int)(columnCount + 0.5)];
            while (columnCurrentCount < columnCount)
            {
                var current = new Rectangle()
                {
                    X = columnValue,
                    Y = rowValue,
                    Height = poolHeight,
                    Width = poolWidth
                };
                columnValue += poolWidth;
                callback(current);
                rectangles[columnCurrentCount] = current;
                columnCurrentCount++;
            }
            if (rectangle.Width % poolWidth > 0)
            {
                var current = new Rectangle()
                {
                    X = columnValue,
                    Y = rowValue,
                    Height = poolHeight,
                    Width = rectangle.Width - columnValue
                };
                callback(current);
                rectangles[columnCurrentCount] = current;
            }
            rowValue += poolHeight;
            rowCurrentCount++;
            while (rowCurrentCount < rowCount)
            {
                Rectangle item;
                for (int i = 0; i < rectangles.Length; i++)
                {
                    item = rectangles[i];
                    item.Y = rowValue;
                    callback(item);
                }
                rowValue += poolHeight;
                rowCurrentCount++;
            }
            if (rectangle.Height % poolHeight > 0)
            {
                Rectangle item;
                int height = rectangle.Height - rowValue;
                for (int i = 0; i < rectangles.Length; i++)
                {
                    item = rectangles[i];
                    item.Y = rowValue;
                    item.Height = height;
                    callback(item);
                }
            }
        }
    }


}
