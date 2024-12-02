using OptimizationTechniques.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Algorithms
{
    public class TIOptimizedAlgorithm : DistanceCalculationsAlgorithm
    {
        public TIOptimizedAlgorithm() : base()
        {
        }

        public TIOptimizedAlgorithm(AlgorithmParams algorithmParams) : base(algorithmParams)
        {
        }

        protected override void processNonSamplePoint(SortedSet<Tuple<double, int>> candidates, int pointIndex)
        {
            var point = X[pointIndex];
            var minIndex = 0;
            var minDistance = Distance.Distance(point, X[SampleIndexes[0]]);
#if METRICS
            _distanceCalculations++;
#endif
            for (var i = 1; i < _samplesCount; i++)
            {
                var sDistance = _sampleDistances[minIndex, i];
                if (sDistance > minDistance * 2) continue;

                var newDistance = Distance.Distance(point, X[SampleIndexes[i]]);
                if (newDistance < minDistance)
                {
                    minIndex = i;
                    minDistance = newDistance;
                }
            }

            AddToOutlierCandidatesIfNeeded(candidates, minDistance, pointIndex);

        }
    }
}
