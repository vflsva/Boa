using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class AlgorithmStatisticsComponent : GH_Component
    {

        public AlgorithmStatisticsComponent()
          : base("Algorithm Statistics", "AlgoStat",
            "Provides information about the Algorithm.",
            "Boa", "Utility")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Boa_AlgorithmParameter(), "Algorithm", "A", "Algorithm to analyze", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntervalParameter("Algorithm Dimensions", "Dim", "Dimensions for this algorithm as a Domain", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Chain Dimensions", "Chain", "Dimensions of all algorithms in the chain.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Chain Length", "Length", "Number of algorithms in the algorithm chain.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Boa_Algorithm algorithm = new Boa_Algorithm();

            if (!DA.GetData(0, ref algorithm)) return;

            Interval dimensionDomain = new Interval(algorithm.InputDimension, algorithm.OutputDimension);
            List<int> chainDimensions = algorithm.GetChainDimensions();
 
            DA.SetData(0, dimensionDomain);
            DA.SetDataList(1, chainDimensions);
            DA.SetData(2, chainDimensions.Count - 1);
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid
        {
            get { return new Guid("cb4f50ef-dca1-45cf-896f-b636d4ece8e1"); }
        }
    }
}
