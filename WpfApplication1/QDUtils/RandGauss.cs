using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.QDUtils
{
    public class RandGauss
    {
        Random rand = new Random(); //reuse this if you are generating many
        public float nextGaussF()
        {
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return (float)randStdNormal;
           // double randNormal =  mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        }
        public float nextGuassF(float mean, float stdDev)
        {
            float stdRand = nextGaussF();
            return mean + stdDev * stdRand; 
        }
       
    }
}
