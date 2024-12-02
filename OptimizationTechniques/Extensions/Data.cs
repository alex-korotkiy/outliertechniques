using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationTechniques.Extensions
{
    public static class Data
    {
        public static double[][] ToDoubleArray(this DataFrame df)
        {
            var rowCount = df.Rows.Count;
            var columnCount = df.Columns.Count;

            double[][] result = new double[rowCount][];

            for (var i = 0; i < rowCount; i++)
            {
                result[i] = new double[columnCount];
                for (var j = 0; j < columnCount; j++)
                {
                    result[i][j] = Convert.ToDouble(df[i, j]);
                }
            }

            return result;
        }

        public static T[] ToArray<T>(this DataFrameColumn column)
        {
            var length = column.Length;
            var result = new T[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = (T)column[i];
            }
            return result;
        }

        public static List<T> ToList<T>(this DataFrameColumn column)
        {
            var length = column.Length;
            var result = new List<T>();
            for (var i = 0; i < length; i++)
            {
                result.Add((T)column[i]);
            }
            return result;
        }
    }
}
