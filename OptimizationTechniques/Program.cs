using Accord.Math.Distances;
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
        const string distanceSuffix = "distance";
        const string all = "all";

        static Dictionary<string, BaseDataset> DatasetStorage = new Dictionary<string, BaseDataset>(StringComparer.InvariantCultureIgnoreCase);
        static Dictionary<string, IDistance<double[]>> DistanceStorage = new Dictionary<string, IDistance<double[]>>(StringComparer.InvariantCultureIgnoreCase);

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

        static void RegisterDistance<T>() where T: IDistance<double[]>, new()
        {
            var key = typeof(T).Name.ToLower();
            DistanceStorage[key] = new T();
        }

        static void RegisterDistances()
        {
            RegisterDistance<Euclidean>();
            RegisterDistance<Manhattan>();
            RegisterDistance<Chebyshev>();
        }


        static void ProcessDatasetWithDistance(BaseDataset dataset, IDistance<double[]> distance, string outputPath)
        {
            var algorithmParams = new AlgorithmParams
            {
                X = dataset.GetX(),
                OutliersCount = dataset.OutliersCount,
                Distance = distance
            };

            var sampleCounts = new int[] { 10, 15, 20, 30, 50, 70, 100, 150, 200, 300, 500, 700, 1000 };
            var repeats = new int[] { 100, 100, 100, 100, 20, 20, 20, 20, 10, 10, 10, 10, 10 };

            var comparer = new AlgorithmsComparer();

            var cResult = comparer.CompareForReports(algorithmParams, sampleCounts, repeats);
            var outputFileName = $"{dataset.BaseOutputFileName}_{distance.GetType().Name}.csv";
            var fullOutputFileName = Path.Combine(outputPath, outputFileName);
            DataFrame.SaveCsv(cResult, fullOutputFileName);
        }


        static void BaseCompareAlgorithms(string datasetName, string distanceName, string outputPath = "")
        {
            var datasets = datasetName == all ? DatasetStorage.Values.ToArray() : new[] { DatasetStorage[datasetName] };
            var distances = distanceName == all ? DistanceStorage.Values.ToArray() : new[] { DistanceStorage[distanceName] };

            foreach (var dataset in datasets)
            {
                foreach (var distance in distances)
                {
                    ProcessDatasetWithDistance(dataset, distance, outputPath);
                }
            }
        }

        static void ShowSyntax()
        {
            Console.WriteLine("OptimizationTechniques DatasetName DistanceName [ReportPath]");
            Console.WriteLine("");
            
            Console.WriteLine("DatasetName should be one of the following:");
            foreach(var k in DatasetStorage.Keys)
            {
                Console.WriteLine(k);
            }
            Console.WriteLine("All");
            Console.WriteLine("Specifying All will process all datasets");
            Console.WriteLine("");

            Console.WriteLine("DistanceName should be one of the following:");
            foreach (var k in DistanceStorage.Keys)
            {
                Console.WriteLine(k);
            }
            Console.WriteLine("All");
            Console.WriteLine("Specifying All will process all distances");
            Console.WriteLine("");

        }

        static void Main(string[] args)
        {
            RegisterDatasets();
            RegisterDistances();

            if (args.Length < 2)
            {
                ShowSyntax();
                return;
            }

            var datasetName = args[0].Trim().ToLower();
            if(datasetName != all && !DatasetStorage.ContainsKey(datasetName))
            {
                Console.WriteLine("Specify supported DatasetName");
                ShowSyntax();
                return;
            }

            var distanceName = args[1].Trim().ToLower();
            if (distanceName != all && !DistanceStorage.ContainsKey(distanceName))
            {
                Console.WriteLine("Specify supported DistanceName");
                ShowSyntax();
                return;
            }

            var reportPath = args.Length < 3  ? string.Empty : args[2];

            BaseCompareAlgorithms(datasetName, distanceName, reportPath);

            Console.ReadLine();
        }
    }
}
