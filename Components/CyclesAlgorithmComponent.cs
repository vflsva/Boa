using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class CyclesAlgorithmComponent : Boa_AlgorithmComponent
    {
        private double cycles;
        private double cycleOffset;

        public CyclesAlgorithmComponent()
          : base("Cycles Algorithm", "Cycles",
            "Algorithm that cycles its inputs between 0 and 1.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Cycles", "C", "Number of times to cycle the inputs", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Offset", "O", "Amount to offset the inputs before cycling", GH_ParamAccess.item, 0.0);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref cycles)) return;
            if (!DA.GetData(1, ref cycleOffset)) return;

            CreateAndSetAlgorithm(DA, Cycles);
        }

        public double[] Cycles (double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                double scaledValue = inputs[i] * cycles + cycleOffset;

                /*should turn this:  /      into this: /\/
                 *                  /
                 *                 /
                 */
                inputs[i] = (Math.Abs(scaledValue / 2 - Math.Round(scaledValue / 2)) * 2);
            }
            return inputs;
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid
        {
            get { return new Guid("91861af6-b525-4765-9816-e6be488c6c2d"); }
        }
    }
}
