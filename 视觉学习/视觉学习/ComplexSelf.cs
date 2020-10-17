using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 复数
    /// </summary>
    public class ComplexSelf
    {
        public double Re { get; set; }
        public double Im { get; set; }
        public double Modulus { get; set; }
        public ComplexSelf(double re, double im)
        {
            Re = re;
            Im = im;
            Modulus = 0;
            //Modules();
        }
        public ComplexSelf()
        {

        }
        public static ComplexSelf operator +(ComplexSelf a, ComplexSelf b)
        {
            a.Re = a.Re + b.Re;
            a.Im = a.Im + b.Im;
            return a;
        }
        public static ComplexSelf operator -(ComplexSelf a, ComplexSelf b)
        {
            a.Re = a.Re - b.Re;
            a.Im = a.Im - b.Im;
            return a;
        }
        public static ComplexSelf operator *(ComplexSelf a, ComplexSelf b)
        {
            var aRe = a.Re;
            var aIm = a.Im;
            var bRe = b.Re;
            var bIm = b.Im;
            a.Re = aRe * bRe - aIm * bIm;
            a.Im = aRe * bIm + aIm * bRe;
            return a;
        }
        public static ComplexSelf operator *(ComplexSelf a, double value)
        {
            a.Re *= value;
            a.Im *= value;
            return a;
        }
        public static ComplexSelf operator /(ComplexSelf a, ComplexSelf b)
        {
            var aRe = a.Re;
            var aIm = a.Im;
            var bRe = b.Re;
            var bIm = b.Im;
            var tmp = bRe * bRe + bIm * bIm;
            a.Re = (aRe * bRe + aIm * bIm) / tmp;
            a.Im = (bRe * aIm - aRe * bIm) / tmp;
            return a;
        }
        public double Modules()
        {
            Modulus = Math.Sqrt(Re * Re + Im * Im);
            return Modulus;
        }
        public void Clear()
        {
            Re = 0;
            Im = 0;
            Modulus = 0;
        }

        public static ComplexSelf ExpBase(int index, int n)
        {
            var remainder = index % n;
            //0/n
            if (remainder == 0)
            {
                return new ComplexSelf(1, 0);
            }
            //判定取一半，1/2
            var halfN = n >> 1;
            if (remainder == halfN)
            {
                return new ComplexSelf(-1, 0);
            }
            //再取一半  1/4
            halfN = halfN >> 1;
            if (remainder == halfN)
            {
                return new ComplexSelf(0, -1);
            }
            //3/4
            if (remainder == (halfN + halfN + halfN))
            {
                return new ComplexSelf(0, 1);
            }
            //   e^(-2πi/n)
            var halfAngle = -Math.PI * remainder / n;
            var y = halfAngle + halfAngle;
            return new ComplexSelf(Math.Cos(y), Math.Sin(y));
        }
    }
}
