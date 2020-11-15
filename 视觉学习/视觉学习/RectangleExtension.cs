using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public static class RectangleExtension
    {
        public static bool CheckOutBitmapRangeAndFit(this Rectangle rectangle, Bitmap bitmap)
        {
            bool flag = true;
            if (rectangle.X < 0)
            {
                rectangle.X = 0;
                flag = false;
            }
            if (rectangle.Y < 0)
            {
                rectangle.Y = 0; flag = false;
            }
            if (rectangle.Right > bitmap.Width - 1)
            {
                rectangle.Width = bitmap.Width - rectangle.X; flag = false;
            }
            if (rectangle.Bottom > bitmap.Height - 1)
            {
                rectangle.Height = bitmap.Height - rectangle.Y; flag = false;
            }
            return flag;
        }
        public static bool CheckOutBitmapRange(this Rectangle rectangle, Bitmap bitmap)
        {
            if (rectangle.X < 0)
            {
                return false;
            }
            if (rectangle.Y < 0)
            {
                return false;
            }
            if (rectangle.Right > bitmap.Width - 1)
            {
                return false;
            }
            if (rectangle.Bottom > bitmap.Height - 1)
            {
                return false;
            }
            return true;
        }
    }
}
