using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视觉学习
{
    public static class FunctionalExperssionImageHandle
    {
        public static void ChangeChannelOrder(Action<Action<int, int, byte, byte, byte, Action<byte, byte, byte>>> imageHandle, Action<byte, byte, byte, Action<byte, byte, byte>> orderCallback)
        {
            imageHandle((x, y, r, g, b, setter) =>
            {
                orderCallback(r, g, b, setter);
            });
        }
        public static void Grayscale(Action<Action<int, int, byte, byte, byte, Action<byte, byte, byte>>> imageHandle)
        {
            imageHandle((x, y, r, g, b, setter) =>
            {
                byte tmp = BitmapHandleExtension.RgbToGray(r, g, b);
                setter(tmp, tmp, tmp);
            });
        }
    }
}
