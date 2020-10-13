using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class MultiplicationAlgorithmComponent : Boa_AlgorithmComponent
    {
        private double numberToMultiply = 1;

        public MultiplicationAlgorithmComponent()
          : base("Multiplication Algorithm", "Alg×X",
            "Algorithm that multiplies its inputs by a number.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Number", "N", "Number to multiply algorithm inputs by", GH_ParamAccess.item, numberToMultiply);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double factor = 0;

            //if (!DA.GetData(0, ref numberToMultiply)) return;
            if (!DA.GetData(0, ref factor)) return;

            var multiplicationSolution = new MultiplicationAlgorithm(factor);
            CreateAndSetAlgorithm(DA, multiplicationSolution.Solve);
            //CreateAndSetAlgorithm(DA, MultiplyNumbers);
        }

        public double[] MultiplyNumbers(double[] inputs)
        {
            double[] results = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
                results[i] = inputs[i] * numberToMultiply;

            return results;
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.multiplicationAlgorithmIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("49b3f602-6671-4424-8cfc-dc03a0f69d4a"); }
        }
    }

    public class MultiplicationAlgorithm : IBoa_Solution
    {
        //private double[] factor;
        private double factor;

        public MultiplicationAlgorithm(double factor)
        {
            this.factor = factor;
            //this.factor = new double[factor.Length];
            //factor.CopyTo(this.factor, 0);
        }

        public double[] Solve(double[] inputs)
        {
            double[] results = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
                results[i] = inputs[i] * factor;//[i];

            return results;
        }
    }
}
