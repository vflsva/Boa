using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class ReverseAlgorithmComponent : Boa_AlgorithmComponent
    {
        /// <summary>
        /// Initializes a new instance of the ReverseAlgorithmComponent class.
        /// </summary>
        public ReverseAlgorithmComponent()
          : base("Reverse Algorithm", "Rev",
              "Algorithm that reverses the order of its inputs.",
              "Boa", "Pattern")
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            CreateAndSetAlgorithm(DA, ReverseAlgorithm);
        }

        public double[] ReverseAlgorithm(double[] inputs)
        {
            double[] outputs = new double[inputs.Length];

            int oI = outputs.Length;

            foreach (double i in inputs)
            {
                oI -= 1;
                outputs[oI] = i;
            }

            return outputs;
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.reverseAlgorithmIcon;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c0299ded-d3af-44fa-87d9-021240859301"); }
        }
    }
}