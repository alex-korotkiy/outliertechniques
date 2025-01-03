using OptimizationTechniques.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Dto.Datasets
{
    public class Shuttle_Dataset : BaseDataset
    {

        public Shuttle_Dataset()
        {
            inputFileName = "Shuttle_normalized.csv";
            baseOutputFileName = "Shuttle";
        }

        public override string Name
        {
            get
            {
                return "Shuttle";
            }
        }

        public override double OutliersCount
        {
            get
            {
                return 2644;
            }
        }

        public override double[][] GetX()
        {
            var df = ReadFile();
            var colCount = df.Columns.Count;
            df.Columns.RemoveAt(colCount - 1);

            return df.ToDoubleArray();
        }
    }
}
