using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class MultiplyAlgorithmsComponent : Boa_ParallelAlgorithmComponent
    {
        /// <summary>
        /// Initializes a new instance of the MultiplyAlgorithmsComponent class.
        /// </summary>
        public MultiplyAlgorithmsComponent()
          : base("Multiply Algorithms", "A1×A2",
              "Multiplies the outputs of two algorithms together.")
        {
        }

        protected override ParallelAlgorithm GenerateParallelAlgorithm(Boa_Algorithm algorithmA, Boa_Algorithm algorithmB)
        {
            return new MultiplyAlgorithms(algorithmA, algorithmB);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.multiplyAlgorithmsIcon;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d3871dbd-049e-4310-a9fa-e4fa47a5c8cd"); }
        }
    }

    public class MultiplyAlgorithms : ParallelAlgorithm
    {
        public MultiplyAlgorithms(Boa_Algorithm algorithmA, Boa_Algorithm algorithmB) : base(algorithmA, algorithmB)
        {
        }

        protected override bool CalculateFixedAlgorithmOutputSize(int size1, int size2, ref int outputSize)
        {
            if (size1 != size2)
            {
                //AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Algorithms outputs are of different sizes and cannot be multiplied.");
                return false;
            }

            outputSize = size1;

            return true;
        }

        protected override double[] SolveParallelAlgorithm(double[] inputsA, double[] inputsB)
        {
            double[] outputs = new double[Math.Min(inputsA.Length, inputsB.Length)];
            for (int i = 0; i < outputs.Length; i++)
                outputs[i] = inputsA[i] * inputsB[i];

            return outputs;
        }
    }
}