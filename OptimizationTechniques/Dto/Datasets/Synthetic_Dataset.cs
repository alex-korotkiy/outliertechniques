using Accord.Statistics.Distributions.Univariate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Dto.Datasets
{
    public class Synthetic_Dataset : BaseDataset
    {

        public Synthetic_Dataset()
        {
            outputFileName = "Synthetic.csv";
        }

        public override string Name => "Synthetic";

        protected int DataPointsCount = 100000;
        public override double OutliersCount => 1000;

        public override double[][] GetX()
        {
            var dimension = 100;
            var centersCount = 10;
            var centersStdev = 1;
            var devsStdev = centersStdev * 0.1;

            var centers = new double[centersCount][];
            var result = new double[DataPointsCount][];

            var devs = NormalDistribution.Random(0, devsStdev, centersCount).Select(Math.Abs).ToArray();

            for (var i = 0; i < centersCount; i++)
            {
                centers[i] = NormalDistribution.Random(0, centersStdev, dimension);
            }

            for (var i = 0; i < DataPointsCount; i++)
            {
                var centerNumber = UniformDiscreteDistribution.Random(0, centersCount);
                var nVector = NormalDistribution.Random(0, devs[centerNumber], dimension);
                for (var j = 0; j < dimension; j++)
                {
                    nVector[j] = nVector[j] + centers[centerNumber][j];
                }
                result[i] = nVector;
            }

            return result;
        }
    }
}
