using Accord.Math.Distances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Dto
{
    public class AlgorithmParams
    {
        public const double defaultOutliersCount = 0.01;
        public const int defaultSamplesCount = 20;

        protected IDistance<double[]> distance = new Euclidean();
        protected double outliersCount = defaultOutliersCount;
        protected int samplesCount = defaultSamplesCount;
        private int[] sampleIndexes;

        public double[][] X { get; set; }

        public double OutliersCount
        {
            get
            {
                return outliersCount;
            }
            set
            {
                if (value <= 0) outliersCount = defaultOutliersCount; else outliersCount = value;
            }
        }
        public int SamplesCount
        {
            get
            {
                return samplesCount;
            }
            set
            {
                if (value <= 0) samplesCount = defaultSamplesCount; else samplesCount = value;
            }
        }

        public int[] SampleIndexes
        { 
            get => sampleIndexes;
            set
            {
                sampleIndexes = value;
                samplesCount = sampleIndexes.Length;
            }
                
        }

        public IDistance<double[]> Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }

        public AlgorithmParams Clone()
        {
            return MemberwiseClone() as AlgorithmParams;
        }

    }
}
