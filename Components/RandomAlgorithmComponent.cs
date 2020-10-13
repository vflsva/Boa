using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class RandomAlgorithmComponent : Boa_AlgorithmComponent
    {
        public RandomAlgorithmComponent()
          : base("Random Algorithm", "Rand",
              "Algorithm that generates a random number based on an input seed.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("Range", "R", "Range in which to generate random numbers", GH_ParamAccess.item, new Interval(0, 1));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Interval range = new Interval(0, 0);

            if (!DA.GetData(0, ref range)) return;

            var randomSolution = new RandomAlgorithm(range);
            CreateAndSetAlgorithm(DA, randomSolution.Solve);
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid
        {
            get { return new Guid("3607c559-9e95-4312-82a0-4f8be17ca9c7"); }
        }


        public class RandomAlgorithm : IBoa_Solution
        {
            private Interval range;

            public RandomAlgorithm(Interval range)
            {
                this.range = range;
            }

            public double[] Solve(double[] inputs)
            {
                double[] results = new double[inputs.Length];

                for (int i = 0; i < inputs.Length; i++)
                    results[i] = new System.Random((int)Math.Round(inputs[i])).NextDouble() * range.Length + range.T0;

                return results;
            }
        }
    }
}