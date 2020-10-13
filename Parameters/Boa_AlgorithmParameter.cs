using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class Boa_AlgorithmParameter : GH_Param<Boa_Algorithm>
    {
        public Boa_AlgorithmParameter()
          : base("Boa Algorithm", "Algo",
            "Parameter that stores a Boa Algorithm.",
            "Params", "Primitive", GH_ParamAccess.item)
        {
            //data
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.algorithmIcon;// Properties.Resources.AlgorithmIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("f1b6ca48-e911-4bb9-9004-fbc43a88d77d"); }
        }
    }
}
