using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Dto.Datasets
{
    public abstract class BaseDataset
    {
        protected string inputFileName;
        protected string baseOutputFileName;

        public abstract string Name { get; }
        public abstract double OutliersCount { get; }
        public abstract double[][] GetX();

        protected virtual DataFrame ReadFile()
        {
            return DataFrame.LoadCsv("Data\\" + inputFileName, ',', false);
        }

        public virtual string BaseOutputFileName
        {
            get
            {
                return baseOutputFileName;
            }
        }
    }
}
