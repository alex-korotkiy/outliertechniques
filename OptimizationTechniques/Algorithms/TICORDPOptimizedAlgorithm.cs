using OptimizationTechniques.Dto;
using OptimizationTechniques.Extensions;
using OptimizationTechniques.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Algorithms
{
    public class TICORDPOptimizedAlgorithm : DPOptimizedAlgorithm
    {
        public TICORDPOptimizedAlgorithm() : base()
        {
        }

        public TICORDPOptimizedAlgorithm(AlgorithmParams algorithmParams) : base(algorithmParams)
        {
        }

        protected override void processNonSamplePoint(SortedSet<Tuple<double, int>> candidates, int pointIndex)
        {
            var point = X[pointIndex];
            var minIndex = 0;
            var pointValue = MathFunctions.Dot(point, point) / 2;
#if METRICS
            _distanceCalculations++;
#endif
            var sample = X[SampleIndexes[0]];
            var minComparedValue = _sampleDValues[0] - MathFunctions.Dot(point, sample);
#if METRICS
            _distanceCalculations++;
#endif
            var minDistanceFunctionValue = minComparedValue + pointValue;
            var candidatesMin = candidates.Min;

            if (candidatesMin != null && minDistanceFunctionValue < candidatesMin.Item1) return;

            for (var i = 1; i < _samplesCount; i++)
            {
                if (_sampleDistances[minIndex, i] >= minDistanceFunctionValue * 4) continue;

                sample = X[SampleIndexes[i]];

                var newValue = _sampleDValues[i] - MathFunctions.Dot(point, sample);
#if METRICS
                _distanceCalculations++;
#endif
                if (newValue < minComparedValue)
                {
                    minIndex = i;
                    minComparedValue = newValue;
                    minDistanceFunctionValue = minComparedValue + pointValue;
                    candidatesMin = candidates.Min;
                    if (candidatesMin != null && minDistanceFunctionValue < candidatesMin.Item1) return;
                }
            }

            AddToOutlierCandidatesIfNeeded(candidates, minDistanceFunctionValue, pointIndex);
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
