using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    /// <summary>
    /// Base class for all Boa_Algorithm solver components.
    /// </summary>
    public abstract class Boa_AlgorithmSolverComponent : GH_Component
    {
        //protected Boa_Algorithm algorithm = new Boa_Algorithm();
        //protected double[] inputs;// = new double[];

        public Boa_AlgorithmSolverComponent(string name, string nickname, string description)
          : base(name, nickname,
            description,
            "Boa", "Solvers")
        {
        }

        public Boa_AlgorithmSolverComponent(string name, string nickname, string description, string category, string subcategory)
          : base(name, nickname,
            description,
            category, subcategory)
        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new Boa_AlgorithmSolverAttributes(this);
        }

        

        protected void AddAlgorithmInput(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(CreateAlgorithmInputParam());
        }

        protected void AddNumbersInput(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Inputs", "I", "Inputs as a list of numbers.", GH_ParamAccess.list);
        }

        protected Boa_AlgorithmParameter CreateAlgorithmInputParam()
        {
            Boa_AlgorithmParameter algorithmParameter = new Boa_AlgorithmParameter();
            algorithmParameter.Name = "Algorithm";
            algorithmParameter.NickName = "A";
            algorithmParameter.Description = "Algorithm to evaluate using supplied inputs.";
            algorithmParameter.Access = GH_ParamAccess.item;
            algorithmParameter.Optional = true;
            return algorithmParameter;
        }

        protected bool GetInputs(IGH_DataAccess DA, ref double[] inputs)
        {
            var inputList = new List<double>(inputs);
            if (!DA.GetDataList(1, inputList))
                return false;

            inputs = inputList.ToArray();
            return true;
        }

        protected bool GetAlgorithm(IGH_DataAccess DA, ref Boa_Algorithm algorithm)
        {
            if (!DA.GetData(0, ref algorithm))
            {
                algorithm = Boa_Algorithm.Default;
                return false;
            }
            return true;
        }

        protected bool SolveAlgorithm (ref double[] inputs, Boa_Algorithm algorithm, string errorMessage)
        {
            SolutionRemark remark = 0;

            inputs = algorithm.Solve(inputs, out remark);

            if (remark.Equals(SolutionRemark.OversizedInputs))
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Length of input list is oversized. Some values were ignored.");

            else if (remark.Equals(SolutionRemark.UndersizedInputs))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, errorMessage);
                return false;
            }

            return true;
        }

        protected bool SolveAlgorithm(ref double[] inputs, Boa_Algorithm algorithm)
        {
            return SolveAlgorithm(ref inputs, algorithm, "Length of input list is undersized. Solution was aborted.");
        }

        //protected override System.Drawing.Bitmap Icon => Properties.Resources.AlgorithmSolverIcon;

        //public override Guid ComponentGuid
        //{
        //    get { return new Guid("2f5986be-0303-40da-af8d-f5a89b67987e"); }
        //}
    }
}
