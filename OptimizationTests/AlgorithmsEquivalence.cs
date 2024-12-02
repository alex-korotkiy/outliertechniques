using Accord.Statistics.Distributions.Univariate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimizationTechniques.Algorithms;
using OptimizationTechniques.Dto;
using System;

namespace OptimizationTests
{
    [TestClass]
    public class AlgorithmsEquivalence
    {
        [TestMethod]
        public void AllAlgorithmsShouldReturnTheSameOutliers()
        {
            int length = 1000000;
            int dimension = 100;
            var x = new double[length][];
            for (var i = 0; i < length; i++)
            {
                x[i] = NormalDistribution.Random(0, 1, dimension);
            }

            var algorithmParams = new AlgorithmParams { X = x };
            algorithmParams.SampleIndexes = BaseAlgorithm.GenerateSamples(100, x.Length);
            var baseAlgorithm = new BaseAlgorithm(algorithmParams);

            var baseIndexes = baseAlgorithm.FitTransform();

            var tiOptimizedAlgorithm = new TIOptimizedAlgorithm(algorithmParams);
            var tiIndexes = tiOptimizedAlgorithm.FitTransform();

            var corAlgorithm = new COROptimizedAlgorithm(algorithmParams);
            var corIndexes = corAlgorithm.FitTransform();

            var ticorAlgorithm = new TICOROptimizedAlgorithm(algorithmParams);
            var ticorIndexes = ticorAlgorithm.FitTransform();

            var dpAlgorithm = new DPOptimizedAlgorithm(algorithmParams);
            var dpIndexes = dpAlgorithm.FitTransform();

            var ticordpAlgorithm = new TICORDPOptimizedAlgorithm(algorithmParams);
            var ticordpIndexes = ticordpAlgorithm.FitTransform();

            Assert.AreEqual(tiIndexes.Length, baseIndexes.Length);
            Assert.AreEqual(corIndexes.Length, baseIndexes.Length);
            Assert.AreEqual(ticorIndexes.Length, baseIndexes.Length);
            Assert.AreEqual(dpIndexes.Length, baseIndexes.Length);
            Assert.AreEqual(ticordpIndexes.Length, baseIndexes.Length);

            for (var i = 1; i < baseIndexes.Length; i++)
            {
                Assert.AreEqual(tiIndexes[i], baseIndexes[i]);
                Assert.AreEqual(corIndexes[i], baseIndexes[i]);
                Assert.AreEqual(ticorIndexes[i], baseIndexes[i]);
                Assert.AreEqual(dpIndexes[i], baseIndexes[i]);
                Assert.AreEqual(ticordpIndexes[i], baseIndexes[i]);
            }
        }
    }
}
