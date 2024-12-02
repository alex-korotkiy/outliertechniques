using OptimizationTechniques.Dto;
using OptimizationTechniques.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Algorithms
{
    public class COROptimizedAlgorithm : DistanceCalculationsAlgorithm
    {
        public COROptimizedAlgorithm() : base()
        {
        }

        public COROptimizedAlgorithm(AlgorithmParams algorithmParams) : base(algorithmParams)
        {
        }

        protected override void processNonSamplePoint(SortedSet<Tuple<double, int>> candidates, int pointIndex)
        {
            var point = X[pointIndex];
            var minDistance = Distance.Distance(point, X[SampleIndexes[0]]);
#if METRICS
            _distanceCalculations++;
#endif
            var candidatesMin = candidates.Min;
            if (candidatesMin != null && minDistance < candidatesMin.Item1) return;

            for (var i = 1; i < SamplesCount; i++)
            {

                var newDistance = Distance.Distance(point, X[SampleIndexes[i]]);
#if METRICS
                _distanceCalculations++;
#endif
                if (newDistance < minDistance)
                {
                    minDistance = newDistance;
                    candidatesMin = candidates.Min;
                    if (candidatesMin != null && minDistance < candidatesMin.Item1) return;
                }
            }

            AddToOutlierCandidatesIfNeeded(candidates, minDistance, pointIndex);
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
