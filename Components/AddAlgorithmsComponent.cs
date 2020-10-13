using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class AddAlgorithmsComponent : Boa_ParallelAlgorithmComponent
    {
        public AddAlgorithmsComponent()
          : base("Add Algorithms", "A1+A2",
            "Adds the outputs of two algorithms or algorithm chains.")
        {
        }

        protected override ParallelAlgorithm GenerateParallelAlgorithm(Boa_Algorithm algorithmA, Boa_Algorithm algorithmB)
        {
            return new AddAlgorithms(algorithmA, algorithmB);
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.addAlgorithmsIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("709e2468-2002-4c1a-aa08-37701b407253"); }
        }
    }

    public class AddAlgorithms : ParallelAlgorithm
    {
        public AddAlgorithms(Boa_Algorithm algorithmA, Boa_Algorithm algorithmB) : base(algorithmA, algorithmB)
        {
        }

        protected override bool CalculateFixedAlgorithmOutputSize(int size1, int size2, ref int outputSize)
        {
            if (size1 != size2)
            {
                //AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Algorithms outputs are of different sizes and cannot be added.");
                return false;
            }

            outputSize = size1;

            return true;
        }

        protected override double[] SolveParallelAlgorithm(double[] inputsA, double[] inputsB)
        {
            double[] outputs = new double[Math.Min(inputsA.Length, inputsB.Length)];
            for (int i = 0; i < outputs.Length; i++)
                outputs[i] = inputsA[i] + inputsB[i];

            return outputs;
        }
    }
}
