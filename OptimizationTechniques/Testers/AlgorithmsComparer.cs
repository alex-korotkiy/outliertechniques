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

        public static string GetAlgorithmPrefix(Type type)
        {
            var suffix = "_";
            var typeName = type.Name;
            var index = typeName.IndexOf("Optimized");
            if (index > 0) return typeName.Substring(0, index) + suffix;
            index = typeName.IndexOf("Algorithm");
            if (index > 0) return typeName.Substring(0, index) + suffix;
            return typeName + suffix;
        }

        public Dictionary<string, List<double>> CompareOnParams(AlgorithmParams algorithmParams, TestParams testParams)
        {

            var result = new Dictionary<string, List<double>>();

            var testers = new IAlgorithmTester[]
            {
                new AlgorithmTester<BaseAlgorithm>(),
                new AlgorithmTester<TIOptimizedAlgorithm>(),
                new AlgorithmTester<COROptimizedAlgorithm>(),
                new AlgorithmTester<DPOptimizedAlgorithm>(),
                new AlgorithmTester<TICOROptimizedAlgorithm>(),
                new AlgorithmTester<TICORDPOptimizedAlgorithm>()
            };

            var metricResultsArray = testers.Select(t => new Dictionary<string, List<double>>()).ToArray();

            for (var i = 0; i < testParams.Repeats; i++)
            {
                Console.WriteLine($"Running all algorithms with {algorithmParams.SamplesCount} sample(s), pass {i + 1} ...");
                var testLoopParams = algorithmParams.Clone();
                testLoopParams.SampleIndexes = BaseAlgorithm.GenerateSamples(testLoopParams.SamplesCount, testLoopParams.X.Length);
                for(var j = 0; j < testers.Length; j++)
                {
                    testers[j].Test(testLoopParams, metricResultsArray[j]);
                }
                Console.WriteLine($"Algorithms with {algorithmParams.SamplesCount} sample(s), pass {i + 1} finished");
            }

            for(var i = 0; i < testers.Length; i++)
            {
                var prefix = GetAlgorithmPrefix(testers[i].ParameterType());
                MergeWithPrefix(prefix, metricResultsArray[i], metricResultsArray[i].Keys, result);
            }

            return result;
        }

        public Dictionary<string, double> CompareOnParamsRow(AlgorithmParams algorithmParams, TestParams testParams)
        {
            var fullData = CompareOnParams(algorithmParams, testParams);
            var result = CalculateAvgAndSigma(fullData);

            var keys = result.Keys.ToArray();
            var suffix = Metrics.RunTime + Metrics.AvgSuffix;
            var basePrefix = GetAlgorithmPrefix(typeof(BaseAlgorithm));

            foreach (var key in keys)
            {
                var index = key.IndexOf(suffix);
                if (index <= 0) continue;
                var prefix = key.Substring(0, index);
                var baseKey = basePrefix + suffix;
                if (key != baseKey && result.ContainsKey(baseKey))
                    result[prefix + Metrics.RunTimePercentAvgRatioSuffix] = result[key] * 100 / result[baseKey];

            }

            return result;
        }

        public DataFrame CompareForReports(AlgorithmParams baseAlgorithmParams, int[] samplesCounts, int[] repeats)
        {
            var dicts = new List<Dictionary<string, double>>();
            for (var i = 0; i < Math.Min(samplesCounts.Length, repeats.Length); i++)
            {
                Console.WriteLine($"Starting tests with {samplesCounts[i]} sample(s) ...");
                var algorithmParams = baseAlgorithmParams.Clone();
                algorithmParams.SamplesCount = samplesCounts[i];
                var testParams = new TestParams() { Repeats = repeats[i] };
                var rowResult = CompareOnParamsRow(algorithmParams, testParams);
                dicts.Add(rowResult);
                Console.WriteLine($"Tests with {samplesCounts[i]} sample(s) finished");
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
