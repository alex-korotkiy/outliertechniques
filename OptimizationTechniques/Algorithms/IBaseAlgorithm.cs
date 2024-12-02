using OptimizationTechniques.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Algorithms
{
    public interface IBaseAlgorithm
    {
        public void Init(AlgorithmParams algorithmParams);
        public int[] FitTransform();
        public Dictionary<string, double> GetMetrics();
    }
}
