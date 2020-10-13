using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    /// <summary>
    /// Base class for all Boa_Algorithm components.
    /// </summary>
    public class Boa_AlgorithmComponent : GH_Component
    {
        private Boa_Algorithm algorithm = new Boa_Algorithm();

        protected int AlgorithmOutputIndex
        {
            get { return 0; }
        }

        protected int ParentInputIndex
        {
            get { return Params.Input.Count - 1; }
        }
        
        public Boa_Algorithm Algorithm { get { return algorithm; } protected set { algorithm = value; } }

        public Boa_AlgorithmComponent()
          : base("Empty Algorithm", "Empty",
            "Empty Algorithm that passes data straight through. This can be used as a common base for combining two or more algorithms.",
            "Boa", "Basic")
        {
        }

        public Boa_AlgorithmComponent(string name, string nickname, string description, string category, string subcategory)
          : base(name, nickname,
            description,
            category, subcategory)
        {
            Algorithm = new Boa_Algorithm();
        }

        public Boa_AlgorithmComponent(string name, string nickname, string description)
          : this(name, nickname,
            description,
            "Boa", "Basic")
        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new Boa_AlgrithmComponentAttributes(this);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            RegisterAdditionalInputParams(pManager);
            pManager.AddParameter(CreateParentInputParam());
        }

        protected sealed override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(CreateAlgorithmOutputParam());
            RegisterAdditionalOutputParams(pManager);
        }

        protected virtual void RegisterAdditionalInputParams(GH_InputParamManager pManager) { }
        protected virtual void RegisterAdditionalOutputParams(GH_OutputParamManager pManager) { }

        protected Boa_AlgorithmParameter CreateParentInputParam()
        {
            return CreateAlgorithmInputParam("Parent", "A", "Optional algorithm to parent to this one.");
        }

        protected Boa_AlgorithmParameter CreateAlgorithmInputParam(string name, string nickname, string description)
        {
            Boa_AlgorithmParameter algorithmParameter = new Boa_AlgorithmParameter();
            algorithmParameter.Name = name;
            algorithmParameter.NickName = nickname;
            algorithmParameter.Description = description;
            algorithmParameter.Access = GH_ParamAccess.item;
            algorithmParameter.Optional = true;
            return algorithmParameter;
        }

        protected Boa_AlgorithmParameter CreateAlgorithmOutputParam()
        {
            Boa_AlgorithmParameter algorithmParameter = new Boa_AlgorithmParameter();
            algorithmParameter.Name = "Algorithm";
            algorithmParameter.NickName = "A";
            algorithmParameter.Description = "Resulting algorithm";
            algorithmParameter.Access = GH_ParamAccess.item;
            return algorithmParameter;
        }

        protected void CreateAndSetAlgorithm(IGH_DataAccess DA, Boa_Algorithm algorithm)
        {
            Algorithm = algorithm;
            Boa_Algorithm parentAlgorithm = new Boa_Algorithm();

            string errorDescription = "";

            if (DA.GetData(ParentInputIndex, ref parentAlgorithm))
            {

                if (!Algorithm.TryAddParent(parentAlgorithm, out errorDescription))
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, errorDescription);
            }

            DA.SetData(AlgorithmOutputIndex, Algorithm);
        }

        protected void CreateAndSetAlgorithm(IGH_DataAccess DA, SolveAlgorithm solveAlgorithm, int inputSize, int outputSize)
        {
            var algorithm = new Boa_Algorithm(solveAlgorithm, inputSize, outputSize);
            CreateAndSetAlgorithm(DA, algorithm);
        }

        protected void CreateAndSetAlgorithm(IGH_DataAccess DA, SolveAlgorithm solveAlgorithm)
        {
            CreateAndSetAlgorithm(DA, solveAlgorithm, 0, 0);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            CreateAndSetAlgorithm(DA, new Boa_Algorithm());
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.emptyAlgorithmIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("a49b353a-f97d-4dd6-8990-595b3a9a859f"); }
        }
    }
}
