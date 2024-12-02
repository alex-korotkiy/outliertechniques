using OptimizationTechniques.Dto;
using OptimizationTechniques.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Algorithms
{
    public class DistanceCalculationsAlgorithm: BaseAlgorithm
    {

        protected int _distanceCalculations;

        public DistanceCalculationsAlgorithm() : base()
        {
        }

        public DistanceCalculationsAlgorithm(AlgorithmParams algorithmParams) : base(algorithmParams)
        {
        }

        public override Dictionary<string, double> GetMetrics()
        {
#if METRICS
            metrics[Metrics.DistanceCalculationsCount] = _samplesCount * (_samplesCount - 1) / 2 + _distanceCalculations;
#endif
            return metrics;
        }
    }
}
