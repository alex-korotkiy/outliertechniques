using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math.Distances;
using OptimizationTechniques.Dto;
using OptimizationTechniques.Measurements;

namespace OptimizationTechniques.Algorithms
{
    public class BaseAlgorithm : IBaseAlgorithm
    {

        protected int _samplesCount;
        protected int _maxSamplesRange;

        protected Dictionary<string, double> metrics = new Dictionary<string, double>();

        protected IComparer<Tuple<double, int>> _pointsComparer;

        protected double[,] _sampleDistances;

        public double[][] X { get; set; }
        public int OutliersCount { get; set; }
        public int SamplesCount
        {
            get
            {
                return _samplesCount;
            }
            set
            {
                _samplesCount = value;
                _maxSamplesRange = value - 1;
            }
        }
        public IDistance<double[]> Distance { get; set; }

        public virtual int[] SampleIndexes { get; set; }

        protected virtual void buildSampleDistances()
        {
            _sampleDistances = new double[_samplesCount, _samplesCount];
            for (var i = 0; i < _maxSamplesRange; i++)
            {
                for (var j = i + 1; j < _samplesCount; j++)
                {
                    _sampleDistances[i, j] = Distance.Distance(X[SampleIndexes[i]], X[SampleIndexes[j]]);
                }
            }
        }

        protected virtual bool AddToOutlierCandidatesIfNeeded(SortedSet<Tuple<double, int>> candidates, double value, int index)
        {
            var count = candidates.Count;

            var minValue = candidates.Min;
            if (count >= OutliersCount && value <= minValue.Item1) return false;
            candidates.Add(new Tuple<double, int>(value, index));
            if (count >= OutliersCount) candidates.Remove(minValue);
            return true;
        }

        protected virtual IEnumerable<double> enumerateSampleDistances(int sampleNumber)
        {
            for (var i = 0; i < sampleNumber; i++)
                yield return _sampleDistances[i, sampleNumber];

            for (var i = sampleNumber + 1; i < _samplesCount; i++)
                yield return _sampleDistances[sampleNumber, i];
        }

        protected virtual void processSamplePoint(SortedSet<Tuple<double, int>> candidates, int sampleNumber)
        {
            var minDistance = enumerateSampleDistances(sampleNumber).Min();
            var sampleIndex = SampleIndexes[sampleNumber];
            AddToOutlierCandidatesIfNeeded(candidates, minDistance, sampleIndex);
        }

        protected virtual void processNonSamplePoint(SortedSet<Tuple<double, int>> candidates, int pointIndex)
        {
            var point = X[pointIndex];
            var minDistance = SampleIndexes.Select(i => Distance.Distance(point, X[i])).Min();
            AddToOutlierCandidatesIfNeeded(candidates, minDistance, pointIndex);
        }

        public virtual void Init(AlgorithmParams algorithmParams)
        {
            X = algorithmParams.X;

            OutliersCount = (int)(algorithmParams.OutliersCount >= 1 ? Math.Round(algorithmParams.OutliersCount) : Math.Round(algorithmParams.OutliersCount * X.Length));
            if (OutliersCount < 1) OutliersCount = 1;

            SamplesCount = algorithmParams.SamplesCount;
            Distance = algorithmParams.Distance;

            _pointsComparer = Comparer<Tuple<double, int>>.Create(
                (t1, t2) => t1.Item1 < t2.Item1 || (t1.Item1 == t2.Item1 && t1.Item2 < t2.Item2) ? -1 :
                t1.Item1 == t2.Item1 && t1.Item2 == t2.Item2 ? 0 :
                1
            );

            if (algorithmParams.SampleIndexes == null)
            {
                SampleIndexes = GenerateSamples(SamplesCount, X.Length);
            }
            else
            {
                SampleIndexes = algorithmParams.SampleIndexes;
            }

        }

        public BaseAlgorithm()
        {

        }

        public BaseAlgorithm(AlgorithmParams algorithmParams)
        {
            Init(algorithmParams);
        }

        public static int[] GenerateSamples(int samplesCount, int totalCount)
        {
            var resultSet = new SortedSet<int>();
            var random = new Random();
            while (resultSet.Count < samplesCount)
            {
                var index = random.Next(0, totalCount);
                if (!resultSet.Contains(index)) resultSet.Add(index);
            }

            return resultSet.ToArray();
        }

        public virtual int[] FitTransform()
        {
            var length = X.Length;
            var candidates = new SortedSet<Tuple<double, int>>(_pointsComparer);
            buildSampleDistances();

            for (var i = 0; i < _samplesCount; i++) processSamplePoint(candidates, i);

            for (var i = 0; i < SampleIndexes[0]; i++) processNonSamplePoint(candidates, i);

            for (var i = 0; i < _maxSamplesRange - 1; i++)
            {
                var start = SampleIndexes[i] + 1;
                var end = SampleIndexes[i + 1];
                for (var j = start; j < end; j++)
                {
                    processNonSamplePoint(candidates, j);
                }
            }

            for (var i = SampleIndexes[_maxSamplesRange]; i < length; i++) processNonSamplePoint(candidates, i);

            return candidates.Select(v => v.Item2).ToArray();
        }

        public virtual Dictionary<string, double> GetMetrics()
        {
#if METRICS
            metrics[Metrics.DistanceCalculationsCount] = (2 * X.Length - SamplesCount - 1) * SamplesCount / 2;
#endif
            return metrics;
        }
    }
}
