﻿using System;
using System.Collections;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

namespace Boa
{
    public class AlgorithmAdditiveFeedbackSolverComponent : Boa_AlgorithmSolverComponent
    {

        public AlgorithmAdditiveFeedbackSolverComponent()
          : base("Additive Feedback Solver", "Feedback+",
            "Iteratively solves a Boa Algorithm by adding its outputs to its inputs and feeding it back in. WARNING: number of outputs must be greater than or equal to number of inputs.")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            AddAlgorithmInput(pManager);
            AddNumbersInput(pManager);
            pManager.AddIntegerParameter("Iterations", "N", "Number of feedback loops.", GH_ParamAccess.item, 10);
            //pManager.AddNumberParameter("Factor", "Factor", "Multiplication factor.", GH_ParamAccess.item, 1.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Outputs", "O", "Outputs from every iteration of the feedback loop", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int iterations = 0;
            var algorithm = new Boa_Algorithm();
            var inputs = new double[0];

            if (!GetInputs(DA, ref inputs)) return;
            if (!DA.GetData(2, ref iterations)) return;
            GetAlgorithm(DA, ref algorithm);

            if (iterations < 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Number of iterations must be greater than or equal to one.");
                return;
            }

            GH_Structure<GH_Number> outputDataTree = new GH_Structure<GH_Number>();
            GH_Path targetPath = DA.ParameterTargetPath(0);

            //Feedback loop
            for (int i = 0; i < iterations; i++)
            {
                double[] originalInputs = new double[inputs.Length];
                inputs.CopyTo(originalInputs, 0);

                // on the first iteration
                if (i == 0)
                {
                    if (!SolveAlgorithm(ref inputs, algorithm))
                        return;
                }
                else
                {
                    if (!SolveAlgorithm(ref inputs, algorithm, "Number of outputs is less than number of inputs. Algorithm cannot support a feedback loop."))
                        return;
                }

                GH_Number[] outputList = new GH_Number[inputs.Length];

                for (int j = 0; j < inputs.Length; j++)
                {
                    //inputs[j] = (inputs[j] * factor) + originalInputs[j];
                    inputs[j] = inputs[j] + originalInputs[j];
                    outputList[j] = new GH_Number(inputs[j]);
                }

                outputDataTree.AppendRange(outputList, targetPath.AppendElement(i));
            }

            DA.SetDataTree(0, outputDataTree);
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.algorithmAdditiveFeedbackSolverIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("81b12f2e-cfbc-4607-9d89-228481c4e72d"); }
        }
    }
}
