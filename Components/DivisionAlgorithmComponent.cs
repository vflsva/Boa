using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class DivisionAlgorithmComponent : Boa_AlgorithmComponent
    {
        private double numberToDivide = 1;
        private bool inputIsDenominator = false;

        public DivisionAlgorithmComponent()
          : base("Division Algorithm", "Alg/X",
            "Algorithm that divides its inputs by a number.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Number", "N", "Number to divide algorithm inputs by", GH_ParamAccess.item, numberToDivide);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //if (!DA.GetData(0, ref numberToDivide)) return;
            double factor = 0;

            if (!DA.GetData(0, ref factor)) return;

            var divisionAlgorithm = new DivisionAlgorithm(factor, inputIsDenominator);
            //DA.SetData(AlgorithmOutputIndex, algorithm);
            CreateAndSetAlgorithm(DA, divisionAlgorithm.Solve);
            //CreateAndSetAlgorithm(DA, DivideNumbers);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Input is Denominator", ToggleDivideMode, true, inputIsDenominator);
        }

        private void ToggleDivideMode(Object sender, EventArgs e)
        {
            inputIsDenominator = !inputIsDenominator;
            ExpireSolution(true);
        }

        public double[] DivideNumbers(double[] inputs)
        {
            double[] results = new double[inputs.Length];

            if (inputIsDenominator)
                for (int i = 0; i < inputs.Length; i++)
                    results[i] = numberToDivide / inputs[i];

            else
                for (int i = 0; i < inputs.Length; i++)
                    results[i] = inputs[i] / numberToDivide;

            return results;
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.divisionAlgorithmIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("81415f70-1cb5-4b23-9ae5-606e6bc597ed"); }
        }
    }

    public class DivisionAlgorithm : IBoa_Solution
    {
        private bool inputIsDenominator = false;
        private double factor;

        public DivisionAlgorithm (double factor, bool inputIsDenominator)
        {
            this.factor = factor;
            this.inputIsDenominator = inputIsDenominator;
        }

        public double[] Solve(double[] inputs)
        {
            double[] results = new double[inputs.Length];

            if (inputIsDenominator)
                for (int i = 0; i < inputs.Length; i++)
                    results[i] = factor / inputs[i];

            else
                for (int i = 0; i < inputs.Length; i++)
                    results[i] = inputs[i] / factor;

            return results;
        }
    }
}
