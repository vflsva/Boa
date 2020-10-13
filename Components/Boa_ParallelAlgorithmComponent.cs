using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class Boa_ParallelAlgorithmComponent : Boa_AlgorithmComponent
    {
        private readonly string inputAlgorithmName = "Algorithm";

        protected virtual string GetInputAlgorithmDescription()
        {
            return "Algorithm to run in parallel";
        }

        /// <summary>
        /// Initializes a new instance of the ParallelAlgorithmComponent class.
        /// </summary>
        public Boa_ParallelAlgorithmComponent()
          : base("Combine Algorithms", "A1&A2",
             "Combines two algorithms in parallel." +
                " Ex.) A 3:2 algorithm combined with a 1:3:1 will result in a 4:3 algorithm." +
                " A 2:1:3 and 2:4:4 sharing the same base will result in a 2:7 algorithm.",
             "Boa", "Special")
        {
        }

        public Boa_ParallelAlgorithmComponent(string name, string nickname, string description)
          : base(name, nickname,
              description,
              "Boa", "Special")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(CreateAlgorithmInputParam(inputAlgorithmName + "A", "A", GetInputAlgorithmDescription()));
            pManager.AddParameter(CreateAlgorithmInputParam(inputAlgorithmName + "B", "B", GetInputAlgorithmDescription()));
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var inputAlgorithmA = new Boa_Algorithm();
            var inputAlgorithmB = new Boa_Algorithm();
            //baseAlgorithmA = new Boa_Algorithm();
            //baseAlgorithmB = new Boa_Algorithm();

            if (!DA.GetData(0, ref inputAlgorithmA)) return;
            if (!DA.GetData(1, ref inputAlgorithmB)) return;

            var parallelAlgorithm = GenerateParallelAlgorithm(inputAlgorithmA, inputAlgorithmB);

            var algorithm = new Boa_Algorithm();
            if (!parallelAlgorithm.GenerateAlgorithm(ref algorithm))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Algorithms outputs are of different sizes and cannot be combined.");
                return;
            }

            Algorithm = algorithm;
            //baseAlgorithmA = inputAlgorithmA.GetBaseAlgorithm();
            //baseAlgorithmB = inputAlgorithmB.GetBaseAlgorithm();

            //inputSizeA = baseAlgorithmA.InputDimension;
            //inputSizeB = baseAlgorithmB.InputDimension;

            //int newInputSize, newOutputSize = 0;
            //SolveAlgorithm parallelAlgorithmSolution;

            //if (inputAlgorithmA.OutputIsFixedSize && inputAlgorithmB.OutputIsFixedSize)
            //    if (!parallelAlgorithm.CalculateFixedAlgorithmOutputSize(inputAlgorithmA.OutputDimension, inputAlgorithmB.OutputDimension, ref newOutputSize)) return;

            //if (baseAlgorithmA.Equals(baseAlgorithmB))
            //{
            //    newInputSize = baseAlgorithmA.InputDimension;
            //    parallelAlgorithmSolution = SolveParallelAlgorithmWithSameBase;
            //    hasSameBase = true;
            //}
            //else
            //{
            //    //if either algorithms have non-fixed, inputs, then this algorithm will have non-fixed inputs (size 0);
            //    if (!baseAlgorithmA.InputIsFixedSize || !baseAlgorithmB.InputIsFixedSize)
            //    {
            //        newInputSize = 0;

            //        if (baseAlgorithmA.InputIsFixedSize)
            //            parallelAlgorithmSolution = SolveFixedAParallelAlgorithm;
            //        else if (baseAlgorithmB.InputIsFixedSize)
            //            parallelAlgorithmSolution = SolveFixedBParallelAlgorithm;
            //        else
            //            parallelAlgorithmSolution = SolveUnFixedParallelAlgorithm;
            //    }
            //    else
            //    {
            //        newInputSize = baseAlgorithmA.InputDimension + baseAlgorithmB.InputDimension;
            //        parallelAlgorithmSolution = SolveFixedParallelAlgorithm;
            //    }
            //    hasSameBase = false;
            //}

            //Algorithm = new Boa_Algorithm(parallelAlgorithmSolution, newInputSize, newOutputSize);

            //if (hasSameBase)
            //    Algorithm.AddDisjointedBase(baseAlgorithmA);

            DA.SetData(AlgorithmOutputIndex, Algorithm);
        }

        protected virtual ParallelAlgorithm GenerateParallelAlgorithm(Boa_Algorithm algorithmA, Boa_Algorithm algorithmB)
        {
            return new ParallelAlgorithm(algorithmA, algorithmB);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.combineAlgorithmsIcon;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e74c1cc4-c410-40f2-b244-a79b85aa333b"); }
        }
    }

    public class ParallelAlgorithm
    {
        protected Boa_Algorithm inputAlgorithmA, inputAlgorithmB;
        protected int inputSizeA, inputSizeB;
        protected Boa_Algorithm baseAlgorithmA, baseAlgorithmB;

        protected bool hasSameBase = false;

        public ParallelAlgorithm(Boa_Algorithm algorithmA, Boa_Algorithm algorithmB)
        {
            inputAlgorithmA = algorithmA;
            inputAlgorithmB = algorithmB;

            baseAlgorithmA = inputAlgorithmA.GetBaseAlgorithm();
            baseAlgorithmB = inputAlgorithmB.GetBaseAlgorithm();

            inputSizeA = baseAlgorithmA.InputDimension;
            inputSizeB = baseAlgorithmB.InputDimension;
        }

        public bool GenerateAlgorithm(ref Boa_Algorithm algorithm)
        {
            int newInputSize, newOutputSize = 0;
            SolveAlgorithm parallelAlgorithmSolution;

            if (inputAlgorithmA.OutputIsFixedSize && inputAlgorithmB.OutputIsFixedSize)
                if (!CalculateFixedAlgorithmOutputSize(inputAlgorithmA.OutputDimension, inputAlgorithmB.OutputDimension, ref newOutputSize)) return false;

            if (baseAlgorithmA.Equals(baseAlgorithmB))
            {
                newInputSize = baseAlgorithmA.InputDimension;
                parallelAlgorithmSolution = SolveParallelAlgorithmWithSameBase;
                hasSameBase = true;
            }
            else
            {
                //if either algorithms have non-fixed, inputs, then this algorithm will have non-fixed inputs (size 0);
                if (!baseAlgorithmA.InputIsFixedSize || !baseAlgorithmB.InputIsFixedSize)
                {
                    newInputSize = 0;

                    if (baseAlgorithmA.InputIsFixedSize)
                        parallelAlgorithmSolution = SolveFixedAParallelAlgorithm;
                    else if (baseAlgorithmB.InputIsFixedSize)
                        parallelAlgorithmSolution = SolveFixedBParallelAlgorithm;
                    else
                        parallelAlgorithmSolution = SolveUnFixedParallelAlgorithm;
                }
                else
                {
                    newInputSize = baseAlgorithmA.InputDimension + baseAlgorithmB.InputDimension;
                    parallelAlgorithmSolution = SolveFixedParallelAlgorithm;
                }
                hasSameBase = false;
            }

            algorithm = new Boa_Algorithm(parallelAlgorithmSolution, newInputSize, newOutputSize);

            if (hasSameBase)
                algorithm.AddDisjointedBase(baseAlgorithmA);

            return true;
        }

        protected virtual bool CalculateFixedAlgorithmOutputSize(int size1, int size2, ref int outputSize)
        {
            outputSize = size1 + size2;
            return true;
        }

        /// <summary>
        /// Virtual SolveAlgorithm method. By default it simply stacks the two arrays together. 
        /// </summary>
        /// <param name="inputsA"></param>
        /// <param name="inputsB"></param>
        /// <returns></returns>
        protected virtual double[] SolveParallelAlgorithm(double[] inputsA, double[] inputsB)
        {
            double[] outputs = new double[inputsA.Length + inputsB.Length];
            inputsA.CopyTo(outputs, 0);
            inputsB.CopyTo(outputs, inputsA.Length);

            return outputs;
        }

        protected double[] SolveParallelAlgorithmWithSameBase(double[] inputs)
        {
            double[] inputsA = new double[inputs.Length];
            double[] inputsB = new double[inputs.Length];
            inputs.CopyTo(inputsA, 0);
            inputs.CopyTo(inputsB, 0);

            return SolveParallelAlgorithm(inputAlgorithmA.Solve(inputsA), inputAlgorithmB.Solve(inputsB));
        }

        //in this case both inputSizeA and inputSizeB should be 1 or greater
        protected double[] SolveFixedParallelAlgorithm(double[] inputs)
        {
            return SplitArrayABAndSolve(inputs, inputSizeA, inputSizeB);
        }

        protected double[] SolveUnFixedParallelAlgorithm(double[] inputs)
        {
            int size = (int)Math.Floor(inputs.Length / 2.0);
            return SplitArrayABAndSolve(inputs, size, size);
        }

        protected double[] SolveFixedAParallelAlgorithm(double[] inputs)
        {
            return SplitArrayABAndSolve(inputs, inputSizeA, inputs.Length - inputSizeA);
        }

        protected double[] SolveFixedBParallelAlgorithm(double[] inputs)
        {
            return SplitArrayABAndSolve(inputs, inputs.Length - inputSizeB, inputSizeB);
        }

        protected double[] SplitArrayABAndSolve(double[] inputs, int sizeA, int sizeB)
        {
            if (sizeA < 0 || sizeB < 0 || sizeA + sizeB > inputs.Length)
                //return inputs;
                throw new Exception("length of input list is undersized.");

            double[] inputsA = new double[sizeA];
            double[] inputsB = new double[sizeB];

            for (int i = 0; i < sizeA; i++)
                inputsA[i] = inputs[i];

            for (int i = 0; i < sizeB; i++)
                inputsB[i] = inputs[i + sizeA];

            return SolveParallelAlgorithm(inputAlgorithmA.Solve(inputsA), inputAlgorithmB.Solve(inputsB));
        }
    }
}