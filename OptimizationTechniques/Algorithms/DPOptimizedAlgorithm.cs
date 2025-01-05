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
    // Technique, used in this class, appliable to 
    public class DPOptimizedAlgorithm : DistanceCalculationsAlgorithm
    {

        protected double[] _sampleDValues; // this array stores ||S||^2 / 2 for each sample S
        //in this class sample _sampleDistances array will store not distances, but values ||Si-Sj||^2 / 2 for sample pair (Si, Sj), i < j

        public DPOptimizedAlgorithm() : base()
        {
        }

        public DPOptimizedAlgorithm(AlgorithmParams algorithmParams) : base(algorithmParams)
        {
        }

        protected override void buildSampleDistances()
        {
            _sampleDValues = new double[_samplesCount];

            for(var i = 0; i < _samplesCount; i++)
            {
                var sample = X[SampleIndexes[i]];
                _sampleDValues[i] = MathFunctions.Dot(sample, sample) / 2;
            }

            _sampleDistances = new double[_samplesCount, _samplesCount];
            for (var i = 0; i < _maxSamplesRange; i++)
            {
                for (var j = i + 1; j < _samplesCount; j++)
                {
                    _sampleDistances[i, j] = _sampleDValues[i] + _sampleDValues[j] - MathFunctions.Dot(X[SampleIndexes[i]], X[SampleIndexes[j]]);
                }
            }
        }

        protected override void processNonSamplePoint(SortedSet<Tuple<double, int>> candidates, int pointIndex)
        {
            var point = X[pointIndex];
            var sample = X[SampleIndexes[0]]; 
            var minValue = _sampleDValues[0] - MathFunctions.Dot(point, sample);

            for (var i = 1; i < _samplesCount; i++)
            {
                sample = X[SampleIndexes[i]];
                var newValue = _sampleDValues[i] - MathFunctions.Dot(point, sample);
                if (newValue < minValue) minValue = newValue;
            }

            minValue = minValue + MathFunctions.Dot(point, point) / 2;
            AddToOutlierCandidatesIfNeeded(candidates, minValue, pointIndex);
        }

        public override Dictionary<string, double> GetMetrics()
        {
#if METRICS
            metrics[Metrics.DistanceCalculationsCount] = (2 * X.Length - SamplesCount - 1) * SamplesCount / 2 + X.Length;
#endif
            return metrics;
        }
    }
}
