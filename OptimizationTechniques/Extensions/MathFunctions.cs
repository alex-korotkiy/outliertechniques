using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Extensions
{
    public static class MathFunctions
    {
        public static double Dot(double[] x, double[] y)
        {
            var result = x[0] * y[0];
            var length = x.Length;
            for (var i = 1; i < length; i++)
                result += x[i] * y[i];
            return result;
        }
    }
}
