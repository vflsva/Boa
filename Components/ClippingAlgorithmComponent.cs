using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class ClippingAlgorithmComponent : Boa_AlgorithmComponent
    {
        private double lowClip = 0;
        private double highClip = 1;

        public ClippingAlgorithmComponent()
          : base("Clipping Algorithm", "Clipping",
            "Algorithm that clips its inputs.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Low Clip", "Low", "Minimum value to clip to", GH_ParamAccess.item, lowClip);
            pManager.AddNumberParameter("High Clip", "High", "Maximum value to clip to", GH_ParamAccess.item, highClip);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref lowClip)) return;
            if (!DA.GetData(1, ref highClip)) return;

            CreateAndSetAlgorithm(DA, Clip);
        }

        public double[] Clip (double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] < lowClip) inputs[i] = lowClip;
                if (inputs[i] > highClip) inputs[i] = highClip;
            }

            return inputs;
        }

        protected override System.Drawing.Bitmap Icon => null;// Properties.Resources.ClippingAlgorithmIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("5a7c305c-0109-4a0a-978c-064286efb77c"); }
        }
    }
}
