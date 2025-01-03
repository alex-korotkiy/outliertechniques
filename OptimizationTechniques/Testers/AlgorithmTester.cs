using OptimizationTechniques.Algorithms;
using OptimizationTechniques.Measurements;
using OptimizationTechniques.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math.Distances;

namespace OptimizationTechniques.Testers
{
    public class AlgorithmTester<T> : IAlgorithmTester where T: IBaseAlgorithm, new() 
    {

        public virtual bool SupportedParams(AlgorithmParams algorithmParams)
        {
            var algorithmType = ParameterType();
            if (algorithmType == typeof(DPOptimizedAlgorithm) || algorithmType == typeof(TICORDPOptimizedAlgorithm))
                return algorithmParams.Distance.GetType() == typeof(Euclidean);

            return true;
        }

        public void MergeMetricsFromRun(Dictionary<string, List<double>> totalMetrics, Dictionary<string, double> metrics)
        {
            foreach (var key in metrics.Keys)
            {
                if (!totalMetrics.ContainsKey(key) || totalMetrics[key] == null) totalMetrics[key] = new List<double>();
                totalMetrics[key].Add(metrics[key]);
            }
        }

        public Type ParameterType()
        {
            return typeof(T);
        }

        public virtual void Test(AlgorithmParams algorithmParams, Dictionary<string, List<double>> metricsResult)
        {
            if (!SupportedParams(algorithmParams)) return;

            var algorithmName = typeof(T).Name;
    
            Console.WriteLine($"Testing algorithm: {algorithmName}, distance: {algorithmParams.Distance.GetType().Name}, # of samples: {algorithmParams.SamplesCount}");
                
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var algorithm = new T();
            algorithm.Init(algorithmParams);
            algorithm.FitTransform();
            stopwatch.Stop();

            var ticks = stopwatch.Elapsed.Ticks;
            var metrics = algorithm.GetMetrics();
            metrics[Metrics.RunTime] = ticks * 1.0 / Stopwatch.Frequency;

            MergeMetricsFromRun(metricsResult, metrics);
        }
    }
}
