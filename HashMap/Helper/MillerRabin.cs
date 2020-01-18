using System;
using System.Numerics;

namespace HashMap.Helper
{
    public class MillerRabin
    {
        /// <summary>
        /// Using Miller Rabin test for verifing primality of a number
        /// </summary>
        /// <param name="n">Number for checking</param>
        /// <returns></returns>
        public static bool IsSimplyNumber(Int32 n)
        {
            Int32 rounds = (Int32) Math.Ceiling(Math.Log(n, 2));
            if (n == 1 || n == 2 || n == 3)
                return true;
            if (n < 2 || n % 2 == 0)
                return false;

            Int32 t = n - 1, s = 0;

            while (t % 2 == 0)
            {
                t /= 2;
                s++;
            }

            for (Int32 round = 0; round < rounds; round++)
            {
                Int32 a = new Random().Next(2, n - 2);
                BigInteger at = BigInteger.ModPow(a, t, n);

                if (at == 1 || at == n - 1)
                    continue;

                bool success = false;
                for (Int32 r = 0; r < s; r++)
                {
                    BigInteger tr = BigInteger.Pow(2, r);
                    Int32 atrt = (Int32) BigInteger.ModPow(at, tr, n);

                    if (atrt == 1)
                        return false;

                    if (atrt == n-1)
                    {
                        success = true;
                        break;
                    }
                }
                
                if(success)
                    continue;

                return false;
            }

            return true;
        }
    }
}
