using OptimizationTechniques.Algorithms;
using OptimizationTechniques.Dto;
using OptimizationTechniques.Extensions;
using OptimizationTechniques.Measurements;
using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Testers
{
    public class AlgorithmsComparer
    {

        public static List<T3> ApplyFormula<T1, T2, T3>(List<T1> values1, List<T2> values2, Func<T1, T2, T3> formula)
        {
            var result = new List<T3>();
            var count = Math.Min(values1.Count, values2.Count);
            for (var i = 0; i < count; i++)
            {
                result.Add(formula(values1[i], values2[i]));
            }
            return result;
        }

        /*
        public static void CalculateDerivativeMetrics(Dictionary<string, List<double>> metrics)
        {
            var basePrefix = GetAlgorithmPrefix<BaseAlgorithm>();

            var keys = metrics.Keys.ToArray();
            foreach (var key in keys)
            {
                int index = 0;

                index = key.IndexOf(Metrics.RunTime);
                if (index > 0)
                {
                    var algorithmPrefix = key.Substring(0, index);
                    if (metrics.ContainsKey(basePrefix + Metrics.RunTime))
                        metrics[algorithmPrefix + Metrics.RunTimePercent] = ApplyFormula(metrics[key], metrics[basePrefix + Metrics.RunTime], (x, y) => x * 100 / y);
                }

                index = key.IndexOf(Metrics.DistanceCalculationsCount);
                if (index > 0)
                {
                    var algorithmPrefix = key.Substring(0, index);
                    if (metrics.ContainsKey(basePrefix + Metrics.DistanceCalculationsCount))
                        metrics[algorithmPrefix + Metrics.DistanceCalculationsPercent] = ApplyFormula(metrics[key], metrics[basePrefix + Metrics.DistanceCalculationsCount], (x, y) => x * 100 / y);
                }
            }

        }
        */

        public static string GetAlgorithmPrefix<T>()
        {
            var suffix = "_";
            var typeName = typeof(T).Name;
            var index = typeName.IndexOf("Optimized");
            if (index > 0) return typeName.Substring(0, index) + suffix;
            index = typeName.IndexOf("Algorithm");
            if (index > 0) return typeName.Substring(0, index) + suffix;
            return typeName + suffix;
        }

        public static void MergeWithPrefix<T>(string prefix, Dictionary<string, T> sourceDictionary, IEnumerable<string> sourceKeySet, Dictionary<string, T> destinationDictionary)
        {
            
            foreach (var key in sourceKeySet)
            {
                if (!sourceDictionary.ContainsKey(key)) continue;
                var newKey = prefix + key;
                destinationDictionary[newKey] = sourceDictionary[key];
            }
        }

        public static Dictionary<string, double> CalculateAvgAndSigma(Dictionary<string, List<double>> originalData)
        {
            var result = new Dictionary<string, double>();
            foreach (var key in originalData.Keys)
            {
                double avg = 0;
                double sigma = 0;
                var values = originalData[key];
                if (values != null && values.Count > 0)
                {
                    avg = values.Average();
                    if (values.Count > 1) sigma = Math.Sqrt(values.Select(x => (x - avg) * (x - avg)).Sum() / (values.Count - 1));
                }
                result[key + Metrics.AvgSuffix] = avg;
                result[key + Metrics.SigmaSuffix] = sigma;
            }
            return result;
        }

        public Dictionary<string, List<double>> CompareOnParams(AlgorithmParams algorithmParams, TestParams testParams)
        {
            var result = new Dictionary<string, List<double>>();

            var baseTester = new AlgorithmTester<BaseAlgorithm>();
            var baseResult = baseTester.Test(algorithmParams, testParams);

            var tiTester = new AlgorithmTester<TIOptimizedAlgorithm>();
            var tiResult = tiTester.Test(algorithmParams, testParams);

            var corTester = new AlgorithmTester<COROptimizedAlgorithm>();
            var corResult = corTester.Test(algorithmParams, testParams);

            var ticorTester = new AlgorithmTester<TICOROptimizedAlgorithm>();
            var ticorResult = ticorTester.Test(algorithmParams, testParams);

            var dpTester = new AlgorithmTester<DPOptimizedAlgorithm>();
            var dpResult = dpTester.Test(algorithmParams, testParams);

            var ticordpTester = new AlgorithmTester<TICORDPOptimizedAlgorithm>();
            var ticordpResult = ticordpTester.Test(algorithmParams, testParams);

            MergeWithPrefix(GetAlgorithmPrefix<BaseAlgorithm>(), baseResult, baseResult.Keys, result);
            MergeWithPrefix(GetAlgorithmPrefix<TIOptimizedAlgorithm>(), tiResult, tiResult.Keys, result);
            MergeWithPrefix(GetAlgorithmPrefix<COROptimizedAlgorithm>(), corResult, corResult.Keys, result);
            MergeWithPrefix(GetAlgorithmPrefix<TICOROptimizedAlgorithm>(), ticorResult, ticorResult.Keys, result);
            MergeWithPrefix(GetAlgorithmPrefix<DPOptimizedAlgorithm>(), dpResult, dpResult.Keys, result);
            MergeWithPrefix(GetAlgorithmPrefix<TICORDPOptimizedAlgorithm>(), ticordpResult, ticordpResult.Keys, result);

            //CalculateDerivativeMetrics(result);

            return result;
        }

        public Dictionary<string, double> CompareOnParamsRow(AlgorithmParams algorithmParams, TestParams testParams)
        {
            var fullData = CompareOnParams(algorithmParams, testParams);
            var result = CalculateAvgAndSigma(fullData);

            var keys = result.Keys.ToArray();
            var suffix = Metrics.RunTime + Metrics.AvgSuffix;
            var basePrefix = GetAlgorithmPrefix<BaseAlgorithm>();

            foreach (var key in keys)
            {
                var index = key.IndexOf(suffix);
                if (index <= 0) continue;
                var prefix = key.Substring(0, index);
                var baseKey = basePrefix + suffix;
                if (key != baseKey && result.ContainsKey(baseKey))
                    result[prefix + Metrics.RunTimePercentAvgRatio] = result[key] * 100 / result[baseKey];

            }

            return result;
        }

        public DataFrame CompareForReports(AlgorithmParams baseAlgorithmParams, int[] samplesCounts, int[] repeats)
        {
            var dicts = new List<Dictionary<string, double>>();
            for (var i = 0; i < Math.Min(samplesCounts.Length, repeats.Length); i++)
            {
                var algorithmParams = baseAlgorithmParams.Clone();
                algorithmParams.SamplesCount = samplesCounts[i];
                algorithmParams.SampleIndexes = BaseAlgorithm.GenerateSamples(algorithmParams.SamplesCount, algorithmParams.X.Length);
                var testParams = new TestParams() { Repeats = repeats[i] };
                var rowResult = CompareOnParamsRow(algorithmParams, testParams);
                dicts.Add(rowResult);
            }

            var columnNames = dicts[0].Keys;

            List<DataFrameColumn> columnsList = new List<DataFrameColumn>();
            columnsList.Add(new Int32DataFrameColumn(Metrics.Samples, samplesCounts));
            foreach (var columnName in columnNames)
            {
                var columnData = new double[samplesCounts.Length];
                for (var i = 0; i < samplesCounts.Length; i++)
                {
                    columnData[i] = dicts[i][columnName];
                }
                columnsList.Add(new DoubleDataFrameColumn(columnName, columnData));
            }

            return new DataFrame(columnsList);
        }

        public static string CommonSigmaString(double avgValue, double sigmaValue, double sigmaFactor)
        {
            var pmValue = sigmaValue * sigmaFactor;
            return avgValue.ToString() + "±" + pmValue.ToString();
        }


    }
}
