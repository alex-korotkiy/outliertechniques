using OptimizationTechniques.Algorithms;
using OptimizationTechniques.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Testers
{
    interface IAlgorithmTester
    {
        public bool SupportedParams(AlgorithmParams algorithmParams);
        public Type ParameterType();
        public void Test(AlgorithmParams algorithmParams, Dictionary<string, List<double>> metricsResult);
    }
}
