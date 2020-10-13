using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class AlgorithmBasicSolverComponent : Boa_AlgorithmSolverComponent
    {
        public AlgorithmBasicSolverComponent()
          : base("Algorithm Solver", "Solver",
            "Solves a Boa Algorithm using the supplied inputs.")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            AddAlgorithmInput(pManager);
            AddNumbersInput(pManager);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Outputs", "O", "Outputs from the algorithm.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var algorithm = new Boa_Algorithm();
            var inputs = new double[0];

            if (!GetInputs(DA, ref inputs)) return;
            GetAlgorithm(DA, ref algorithm);

            if (!SolveAlgorithm(ref inputs, algorithm)) return;

            DA.SetDataList(0, inputs);
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.algorithmSolverIcon;//Properties.Resources.AlgorithmSolverIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("736dbe8a-6838-4cf9-b038-0b5dd5e697cd"); }
        }
    }
}
