using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class SortAlgorithmComponent : Boa_AlgorithmComponent
    {
        /// <summary>
        /// Initializes a new instance of the SortAlgorithmComponent class.
        /// </summary>
        public SortAlgorithmComponent()
          : base("Sort Algorithm", "Sort",
              "Algorithm that sorts its inputs in ascending order.",
              "Boa", "Pattern")
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            CreateAndSetAlgorithm(DA, SortAlgorithm);
        }

        public double[] SortAlgorithm(double[] inputs)
        {
            List<double> inputList = new List<double>(inputs);
            inputList.Sort();

            return inputList.ToArray();
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.sortAlgorithmIcon;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a8f3171f-4ed2-47e8-a8ec-11f4958d2313"); }
        }
    }
}