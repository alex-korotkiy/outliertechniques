using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Measurements

{
    public static class Metrics
    {
        public const string RunTime = "RunTime";
        public const string RunTimePercent = "RunTimePercent";
        public const string DistanceCalculationsCount = "DistanceCalculationsCount";
        public const string DistanceCalculationsPercent = "DistanceCalculationsPercent";

        public const string Samples = "Samples";

        public const string AvgSuffix = "_Avg";
        public const string SigmaSuffix = "_Sigma";

        public const string RunTimePercentAvgRatioSuffix = "_RunTimePercentAvgRatio";
    }
}
