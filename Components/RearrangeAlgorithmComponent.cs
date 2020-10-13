using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class RearrangeAlgorithmComponent : Boa_AlgorithmComponent
    {
        private readonly int[] defaultPattern = new int[] { 0, 0, 1, 1 };

        public RearrangeAlgorithmComponent()
          : base("Rearrange Algorithm", "Rearrange",
            "Algorithm that rearranges its outputs based on a user-defined pattern.", "Boa", "Pattern")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Pattern", "P", "Pattern used to map inputs to outputs.", GH_ParamAccess.list, defaultPattern);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var patternList = new List<int>();

            if (!DA.GetDataList(0, patternList)) return;

            var rearrangeAlgorithm = new RearrangeAlgorithm(patternList);

            CreateAndSetAlgorithm(DA, rearrangeAlgorithm.SolveAlgorithm, rearrangeAlgorithm.GetInputDimension(), rearrangeAlgorithm.GetOutputDimension());
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.rearrangeAlgorithmIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("1c42755d-911c-4b50-ae0c-946e58cd7dfc"); }
        }
    }

    public class RearrangeAlgorithm : IBoa_AlgorithmSolution
    {
        private int[] pattern;

        public RearrangeAlgorithm(List<int> patternList)
        {
            pattern = patternList.ToArray();
        }

        public int GetInputDimension()
        {
            int highestDimension = 0;

            foreach (int p in pattern)
                if (p + 1 > highestDimension) highestDimension = p + 1;

            return highestDimension;
        }

        public int GetOutputDimension()
        {
            return pattern.Length;
        }

        public double[] SolveAlgorithm(double[] inputs)
        {
            double[] outputs = new double[pattern.Length];

            for (int i = 0; i < outputs.Length; i++)
                outputs[i] = inputs[pattern[i]];

            return outputs;
        }
    }
}
