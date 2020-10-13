using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class AlgorithmSumComponent : Boa_AlgorithmComponent
    {

        public AlgorithmSumComponent()
          : base("Algorithm Sum", "AlgSum",
            "Algorithm that returns the sum of all inputs.", "Boa", "Special")
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            CreateAndSetAlgorithm(DA, Sum, 0, 1);
        }

        public double[] Sum (double[] inputs)
        {
            double sum = 0;
            foreach (double i in inputs)
                sum += i;
            return new double[] { sum };
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.algorithmSumIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("3d0752d0-bd28-4f0f-9842-a12f5fcee558"); }
        }
    }
}
