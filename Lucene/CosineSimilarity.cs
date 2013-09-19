using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene
{
    public static class CosineSimilarity
    {
        public static double Compute(Dictionary<string,int> d1, Dictionary<string, int> d2)
        {
            double d1Magnitude = 0.0;
            foreach (int val in d1.Values)
            {
                d1Magnitude += (val * val); 
            }
            d1Magnitude = Math.Sqrt(d1Magnitude);

            double d2Magnitude = 0.0;
            foreach (int val in d2.Values)
            {
                d2Magnitude += (val * val); 
            }
            d2Magnitude = Math.Sqrt(d2Magnitude);

            double summation = 0.0; 
            foreach (string key in d1.Keys)
            {
                summation += d1[key] * d2[key]; 
            }

            return summation / (d1Magnitude * d2Magnitude); 
        }
    }
}
