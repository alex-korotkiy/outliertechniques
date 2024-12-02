using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Dto
{
    public class TestParams
    {
        public int Repeats { get; set; }

        public TestParams Clone()
        {
            return (TestParams)MemberwiseClone();
        }
    }
}
