using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class RemappingAlgorithmComponent : Boa_AlgorithmComponent
    {
        private Interval from = new Interval(0.0, 1.0);
        private Interval to = new Interval(0.0, 2 * Math.PI);

        public RemappingAlgorithmComponent()
          : base("Remapping Algorithm", "Remap",
            "Algorithm that remaps its inputs.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntervalParameter("Input Domain", "F", "Domain to map from", GH_ParamAccess.item, from);
            pManager.AddIntervalParameter("Output Domain", "T", "Domain to map to", GH_ParamAccess.item, to);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref from)) return;
            if (!DA.GetData(1, ref to)) return;

            CreateAndSetAlgorithm(DA, Remap);
        }

        public double[] Remap(double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                double fromAbs = inputs[i] - from.T0;
                double fromMaxAbs = from.T1 - from.T0;

                double normal = fromAbs / fromMaxAbs;

                double toMaxAbs = to.T1 - to.T0;
                double toAbs = toMaxAbs * normal;

                inputs[i] = toAbs + to.T0;
            }
            return inputs;
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid
        {
            get { return new Guid("3fb19080-a75f-4d3e-b34b-647cdd087744"); }
        }
    }
}
