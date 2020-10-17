using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Complex
    {
        public double Re { get; set; }
        public double Im { get; set; }
        public double Modulus { get; set; }
        public Complex(double re, double im)
        {
            Re = re;
            Im = im;
            Modulus = 0;
            //Modules();
        }
        public Complex()
        {

        }
        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Re + b.Re, a.Im + b.Im);
        }
        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Re - b.Re, a.Im - b.Im);
        }
        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Re * b.Re - a.Im * b.Im, a.Re * b.Im + a.Im * b.Re);
        }
        public static Complex operator *(Complex a, double value)
        {
            return new Complex(a.Re * value, a.Im * value);
        }
        public static Complex operator /(Complex a, Complex b)
        {
            var tmp = b.Re * b.Re + b.Im * b.Im;
            return new Complex((a.Re * b.Re + a.Im * b.Im) / tmp,
                (b.Re * a.Im - a.Re * b.Im) / tmp);
        }
        public double Modules()
        {
            Modulus = Math.Sqrt(Re * Re + Im * Im);
            return Modulus;
        }

        public static Complex ExpBase(int index, int n)
        {
            var remainder = index % n;
            //0/n
            if (remainder == 0)
            {
                return new Complex(1, 0);
            }
            //判定取一半，1/2
            var halfN = n >> 1;
            if (remainder == halfN)
            {
                return new Complex(-1, 0);
            }
            //再取一半  1/4
            halfN = halfN >> 1;
            if (remainder == halfN)
            {
                return new Complex(0, -1);
            }
            //3/4
            if (remainder == (halfN + halfN + halfN))
            {
                return new Complex(0, 1);
            }
            //   e^(-2πi/n)
            var halfAngle = -Math.PI * remainder / n;
            var y = halfAngle + halfAngle;
            return new Complex(Math.Cos(y), Math.Sin(y));
        }
    }
}
