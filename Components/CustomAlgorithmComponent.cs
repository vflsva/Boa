using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Expressions;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;

namespace Boa
{
    public class CustomAlgorithmComponent : Boa_AlgorithmComponent, IGH_VariableParameterComponent
    {
        private readonly string expressionStringParamDescription = "User-defined expression for an input number at index ";

        public CustomAlgorithmComponent()
          : base("Custom Algorithm", "Custom",
            "A symmetrical algorithm that takes an expression for each dimension. 1-dimensional algorithms will be treated as n-dimensional, being able to operate on algorithms of any size output. Use variable 'X' in expressions.",
            "Boa", "Special")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Expression", "F", expressionStringParamDescription + "0", GH_ParamAccess.item);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Output) return false;
            //we do not want our last parameter (parent algorithm) to be variable
            if (index >= Params.Input.Count) return false;
            return true;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Output) return false;
            //we do not want to remove our last parameter
            if (index >= Params.Input.Count - 1) return false;
            //we should always have 1 expression string
            if (Params.Input.Count <= 2) return false;
            return true;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            IGH_Param p = new Param_String();
            p.Name = "Expression";// + index.ToString();
            p.NickName = "F";
            p.Description = expressionStringParamDescription + index.ToString();
            p.Access = GH_ParamAccess.item;
            return p;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            //resetting the descriptions so all params are named with the correct index
            for (int i = 0; i < Params.Input.Count - 1; i++)
                Params.Input[i].Description = expressionStringParamDescription + i.ToString();
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int expressionEndIndex = Params.Input.Count - 2;

            string[] expressionStrings = new string[Params.Input.Count - 1];

            for (int i = 0; i <= expressionEndIndex; i++)
                if (!DA.GetData(i, ref expressionStrings[i])) return;

            var customAlgorithm = new CustomAlgorithm(expressionStrings);

            int dimension = expressionStrings.Length;
            if (expressionStrings.Length == 1)
                dimension = 0;

            CreateAndSetAlgorithm(DA, customAlgorithm.SolveAlgorithm, dimension, dimension);
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.customAlgorithmIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("b2d9dfb3-74d8-418a-8c02-5fd453730111"); }
        }
    }

    public class CustomAlgorithm : IBoa_AlgorithmSolution
    {
        private GH_ExpressionParser[] parsers;

        public CustomAlgorithm(string[] expressionStrings)
        {
            parsers = new GH_ExpressionParser[expressionStrings.Length];

            for (int i = 0; i < expressionStrings.Length; i++)
            {
                parsers[i] = new GH_ExpressionParser();
                parsers[i].CacheSymbols(expressionStrings[i]);
            }
        }

        public double[] SolveAlgorithm(double[] inputs)
        {
            if (parsers.Length == 1)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    parsers[0].AddVariable("X", inputs[i]);
                    inputs[i] = parsers[0].Evaluate()._Double;
                }
                return inputs;
            }

            double[] outputs = new double[parsers.Length];

            for (int i = 0; i < parsers.Length; i++)
            {
                parsers[i].AddVariable("X", inputs[i]);
                outputs[i] = parsers[i].Evaluate()._Double;
            }

            return outputs;
        }
    }
}
