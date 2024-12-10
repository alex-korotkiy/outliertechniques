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

            //var sampleCounts = new int[] { 10, 15, 20};
            //var repeats = new int[] { 10, 10, 10 };

            var comparer = new AlgorithmsComparer();

            var cResult = comparer.CompareForReports(algorithmParams, sampleCounts, repeats);
            var fullOutputFileName = Path.Combine(outputPath, dataset.OutputFileName);
            DataFrame.SaveCsv(cResult, fullOutputFileName);
        }

        static void BaseCompareAlgorithms(string datasetName, string outputPath = "")
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
            Console.WriteLine("OptimizationTechniques ProcessDataset [DatasetName] [ReportPath]");
        }

        static void Main(string[] args)
        {
            if (args.Length < 1 || args[0].ToLower().Trim() != "processdataset")
            {
                ShowSyntax();
                return;
            }

            var dataSetName = args[1];
            var reportPath = args.Length < 3  ? string.Empty : args[2];

            BaseCompareAlgorithms(dataSetName, reportPath);

            Console.ReadLine();
        }
    }
}
