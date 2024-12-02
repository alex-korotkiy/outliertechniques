using OptimizationTechniques.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Algorithms
{
    public class TICOROptimizedAlgorithm : DistanceCalculationsAlgorithm
    {
        public TICOROptimizedAlgorithm() : base()
        {
        }

        public TICOROptimizedAlgorithm(AlgorithmParams algorithmParams) : base(algorithmParams)
        {
        }

        protected override void processNonSamplePoint(SortedSet<Tuple<double, int>> candidates, int pointIndex)
        {
            var point = X[pointIndex];
            var minIndex = 0;
            var minDistance = Distance.Distance(point, X[SampleIndexes[0]]);

            var candidatesMin = candidates.Min;
            if (candidatesMin != null && minDistance < candidatesMin.Item1) return;

            for (var i = 1; i < SamplesCount; i++)
            {
                var sDistance = _sampleDistances[minIndex, i];
                if (sDistance > minDistance * 2) continue;

                var newDistance = Distance.Distance(point, X[SampleIndexes[i]]);
#if METRICS
                _distanceCalculations++;
#endif
                if (newDistance < minDistance)
                {
                    minIndex = i;
                    minDistance = newDistance;
                    candidatesMin = candidates.Min;
                    if (candidatesMin != null && minDistance < candidatesMin.Item1) return;
                }
            }

            AddToOutlierCandidatesIfNeeded(candidates, minDistance, pointIndex);

        }

    }
}
