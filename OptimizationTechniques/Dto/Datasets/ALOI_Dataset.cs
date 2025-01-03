using OptimizationTechniques.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Dto.Datasets
{
    public class ALOI_Dataset : BaseDataset
    {

        public ALOI_Dataset()
        {
            inputFileName = "ALOI_norm.csv";
            baseOutputFileName = "ALOI";
        }

        public override string Name
        {
            get
            {
                return "ALOI";
            }
        }

        public override double OutliersCount
        {
            get
            {
                return 1508;
            }
        }

        public override double[][] GetX()
        {
            var df = ReadFile();
            var colCount = df.Columns.Count;
            df.Columns.RemoveAt(colCount - 1);
            df.Columns.RemoveAt(colCount - 2);

            return df.ToDoubleArray();
        }
    }
}
