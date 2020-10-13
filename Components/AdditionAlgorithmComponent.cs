using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class AdditionAlgorithmComponent : Boa_AlgorithmComponent
    {

        //private List<double> numbersToAdd = new List<double>();
        private double numberToAdd = 1;

        public AdditionAlgorithmComponent()
          : base("Addition Algorithm", "Alg+X",
            "Algorithm that adds a number to its inputs.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Number", "N", "Number to add to algorithm inputs", GH_ParamAccess.item, numberToAdd);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref numberToAdd)) return;
            //if (!DA.GetDataList(0, numbersToAdd)) return;

            CreateAndSetAlgorithm(DA, AddNumbers);
        }

        public double[] AddNumbers (double[] inputs)
        {
            double[] results = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
                results[i] = inputs[i] + numberToAdd;// numbersToAdd[i % numbersToAdd.Count];

            return results;
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.additionAlgorithmIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("7f418516-38ab-4617-90ca-ffef470a9c26"); }
        }
    }
}
