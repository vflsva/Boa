using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class ExplodeAlgorithmComponent : Boa_AlgorithmComponent
    {

        public ExplodeAlgorithmComponent()
          : base("Explode Algorithm", "Nickname",
            "Explodes an Algorithm into ")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        protected override void RegisterAdditionalOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid
        {
            get { return new Guid("be7b2139-dba2-4571-a41b-7fa7040586e7"); }
        }
    }
}
