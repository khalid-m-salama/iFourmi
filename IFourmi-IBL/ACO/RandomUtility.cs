using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.ACO
{
    public static class RandomUtility
    {
        public static long seed = 123456789;

        public static double NextDouble()
        {
            long k;
            double ans;
            long IA = 16807;
            long IM = 2147483647;
            double AM = (1.0 / IM);
            long IQ = 127773;
            long IR = 2836;

            k = (seed) / IQ;
            seed = IA * (seed - k * IQ) - IR * k;
            if (seed < 0) seed += IM;
            ans = AM * (seed);
            return ans;
        }

        public static double Random(double Min, double Max)
        {
            double ans = (NextDouble() * (Max-Min)) + Min;
            return ans;
        }


        public static double Gaussian(double mean, double std)
        {
           
                double gw, gy1 ; 
                double gx1, gx2 ; 
               do {
                        gx1 = 2.0 * NextDouble() - 1.0 ;
                        gx2 = 2.0 * NextDouble() - 1.0; 
                        gw = gx1 * gx1 + gx2 * gx2 ; 
                } while (gw >= 1.0) ;

               gw = Math.Sqrt((-2 * Math.Log(gw)) / gw);
                gy1 = (gx1 * gw) * std + mean  ;

                return gy1 ; 
        }
    }
}
