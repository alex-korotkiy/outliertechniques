using Accord.Statistics.Distributions.Univariate;
using Microsoft.Data.Analysis;
using OptimizationTechniques.Algorithms;
using OptimizationTechniques.Dto;
using OptimizationTechniques.Dto.Datasets;
using OptimizationTechniques.Testers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OptimizationTechniques
{
    class Program
    {

        static Dictionary<string, BaseDataset> DatasetStorage = new Dictionary<string, BaseDataset>(StringComparer.InvariantCultureIgnoreCase);

        static T RegisterDataset<T>() where T : BaseDataset, new()
        {
            var dataset = new T();
            DatasetStorage[dataset.Name] = dataset;
            return dataset;
        }

        static void RegisterDatasets()
        {
            RegisterDataset<ALOI_Dataset>();
            RegisterDataset<ForestCover_Dataset>();
            RegisterDataset<Shuttle_Dataset>();
            RegisterDataset<Satellite_Dataset>();
            RegisterDataset<Synthetic_Dataset>();
        }

        static void ProcessDataset(BaseDataset dataset, string outputPath)
        {
            var algorithmParams = new AlgorithmParams
            {
                X = dataset.GetX(),
                OutliersCount = dataset.OutliersCount
            };


            var sampleCounts = new int[] { 10, 15, 20, 30, 50, 70, 100, 150, 200, 300, 500, 700, 1000 };
            var repeats = new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
            var comparer = new AlgorithmsComparer();

            var cResult = comparer.CompareForReports(algorithmParams, sampleCounts, repeats);
            var fullOutputFileName = Path.Combine(outputPath, dataset.OutputFileName);
            DataFrame.SaveCsv(cResult, fullOutputFileName);
        }

        static void SimpleComparison()
        {
            int length = 10000;
            int dimension = 100;
            var x = new double[length][];
            for (var i = 0; i < length; i++)
            {
                x[i] = NormalDistribution.Random(0, 1, dimension);
            }

            var algorithmParams = new AlgorithmParams { X = x };
            algorithmParams.SampleIndexes = BaseAlgorithm.GenerateSamples(100, x.Length);
            var baseAlgorithm = new BaseAlgorithm(algorithmParams);


            var baseStopwatch = new Stopwatch();
            baseStopwatch.Start();
            var baseIndexes = baseAlgorithm.FitTransform();
            baseStopwatch.Stop();
            var baseTime = baseStopwatch.ElapsedTicks * 1.0 / Stopwatch.Frequency;

            Console.WriteLine($"Base time: {baseTime}");

            var dpOptimizedAlgorithm = new DPOptimizedAlgorithm(algorithmParams);
            var dpStopwatch = new Stopwatch();
            dpStopwatch.Start();
            var dpOptimizedIndexes = dpOptimizedAlgorithm.FitTransform();
            dpStopwatch.Stop();
            var dpTime = dpStopwatch.ElapsedTicks * 1.0 / Stopwatch.Frequency;

            Console.WriteLine($"DP optimized time: {dpTime}");

            var ticorOptimizedAlgorithm = new TICOROptimizedAlgorithm(algorithmParams);
            var ticorStopwatch = new Stopwatch();
            ticorStopwatch.Start();
            var ticorOptimizedIndexes = ticorOptimizedAlgorithm.FitTransform();
            ticorStopwatch.Stop();
            var ticorTime = ticorStopwatch.ElapsedTicks * 1.0 / Stopwatch.Frequency;

            Console.WriteLine($"TICOR optimized time: {ticorTime}");

            var ticordpOptimizedAlgorithm = new TICORDPOptimizedAlgorithm(algorithmParams);
            var ticordpStopwatch = new Stopwatch();
            ticordpStopwatch.Start();
            var ticordpOptimizedIndexes = ticordpOptimizedAlgorithm.FitTransform();
            ticordpStopwatch.Stop();
            var ticordpTime = ticordpStopwatch.ElapsedTicks * 1.0 / Stopwatch.Frequency;

            Console.WriteLine($"TICORDP optimized time: {ticordpTime}");

        }

        static void ComparerTest()
        {
            int length = 10000;
            int dimension = 100;
            var x = new double[length][];
            for (var i = 0; i < length; i++)
            {
                x[i] = NormalDistribution.Random(0, 1, dimension);
            }

            var algorithmParams = new AlgorithmParams { X = x };

            var testParams = new TestParams() { Repeats = 10 };

            var comparer = new AlgorithmsComparer();

            var samplesCounts = new int[] { 10, 20, 50, 100, 200, 500, 1000 };
            var repeats = new int[] { 10, 10, 10, 10, 10, 10, 10 };

            var result = comparer.CompareForReports(algorithmParams, samplesCounts, repeats);

            DataFrame.SaveCsv(result, "compare.csv");
        }

        static void BaseCompareAlgorithms(string outputPath, string datasetName = "")
        {
            RegisterDatasets();

            if (string.IsNullOrEmpty(datasetName))
            {
                foreach (var key in DatasetStorage.Keys)
                {
                    ProcessDataset(DatasetStorage[key], outputPath);
                }
            }
            else
            {
                if (!DatasetStorage.ContainsKey(datasetName))
                {
                    Console.WriteLine("DataSet name should be one of the following supported datasets:");
                    foreach (var key in DatasetStorage.Keys)
                    {
                        Console.WriteLine(key);
                    }
                    return;
                }
                ProcessDataset(DatasetStorage[datasetName], outputPath);
            }
        }

        static void ShowSyntax()
        {
            Console.WriteLine("AlgorithmsOptimization ReportPath [DatasetName]");
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowSyntax();
                return;
            }

            var reportPath = args[0];
            var dataSetName = args.Length == 1 ? string.Empty : args[1];

            BaseCompareAlgorithms(reportPath, dataSetName);

            Console.ReadLine();
        }
    }
}
