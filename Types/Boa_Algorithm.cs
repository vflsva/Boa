using System;
using System.Collections.Generic;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Boa
{

    public delegate double[] SolveAlgorithm(double[] inputs);
    public delegate double FloatAlgorithm31(float x, float y, float z);

    public enum SolutionRemark
    {
        Successful,
        UndersizedInputs,
        OversizedInputs
    }

    public interface IBoa_Solution
    {
        double[] Solve(double[] inputs);
    }

    public class Boa_Algorithm : IGH_Goo
    {
        private bool hasParent = false;
        private bool hasDisjointedBase = false;
        private Boa_Algorithm disjointedBase;

        protected Boa_Algorithm parent;
        //private List<Boa_Algorithm> children;

        //private Boa_Algorithm highestParent;
        protected SolveAlgorithm solveAlgorithm;

        public int InputDimension { get; protected set; }
        public int OutputDimension { get; protected set; }

        public bool InputIsFixedSize { get; protected set; }
        public bool OutputIsFixedSize { get; protected set; }

        public bool HasParent { get { return hasParent; } protected set { hasParent = value; } }
        public bool IsBase { get { return !(hasParent || hasDisjointedBase); } }
        public Boa_Algorithm Parent => parent;

        public static Boa_Algorithm Default = new Boa_Algorithm();

        /// <summary>
        /// Default constructor that creates an empty algorithm.
        /// </summary>
        public Boa_Algorithm() : this(SolveAlgorithmDefault)
        {
        }

        /// <summary>
        /// Use this constructor if your algorithm works on arrays of any length and returns arrays of that same length.
        /// </summary>
        /// <param name="solveAlgorithm"></param>
        public Boa_Algorithm(SolveAlgorithm solveAlgorithm) : this(solveAlgorithm, 0, 0)
        {
        }

        /// <summary>
        /// Use this constructor if your algorithm expects and returns arrays of certain lengths.
        /// </summary>
        /// <param name="solveAlgorithm"></param>
        /// <param name="inputDimension"></param>
        /// <param name="outputDimension"></param>
        public Boa_Algorithm(SolveAlgorithm solveAlgorithm, int inputDimension, int outputDimension)
        {
            //if (inputDimension != 0 )
            TrySetDimensions(inputDimension, outputDimension);
            this.solveAlgorithm = solveAlgorithm;
        }

        //Copy constructor
        public Boa_Algorithm(Boa_Algorithm algorithmSource)
        {
            solveAlgorithm = algorithmSource.solveAlgorithm;
            InputDimension = algorithmSource.InputDimension;
            OutputDimension = algorithmSource.OutputDimension;
            InputIsFixedSize = algorithmSource.InputIsFixedSize;
            OutputIsFixedSize = algorithmSource.OutputIsFixedSize;
            TryAddParent(algorithmSource.parent);
        }

        public bool TryAddParent(Boa_Algorithm parentToAdd)
        {
            return TryAddParent(parentToAdd, out string s);
        }

        public bool TryAddParent(Boa_Algorithm parentToAdd, out string errorDescription)
        {
            if (parentToAdd == null)
            {
                errorDescription = "Parent is null";
                return false;
            }

            if (!parentToAdd.IsValid)
            {
                errorDescription = "Parent " + parentToAdd.IsValidWhyNot;
                return false;
            }

            if (!parentToAdd.CanBeParentOf(this))
            {
                errorDescription = "Algorithms are of incompatible size";
                return false;
            }

            //if parent's output is not fixed
            if (!parentToAdd.OutputIsFixedSize)
            {
                if (!parentToAdd.TryMatchOutputDimension(InputDimension))
                {
                    errorDescription = "Parenting would result in an input size of 0 or less";
                    return false;
                }
            }

            //if this input is not fixed
            else if (!InputIsFixedSize)
            {
                //if this algorithm's size is relative but the parent's size is fixed
                if (!TryMatchInputDimension(parentToAdd.OutputDimension))
                {
                    errorDescription = "Parenting would result in an output size of 0 or less";
                    return false;
                }
            }

            parent = parentToAdd;
            HasParent = true;
            errorDescription = "Algorithm has been successfully parented";

            return true;
        }

        /// <summary>
        /// Checks to see if this algorithm can be made the parent of another.
        /// </summary>
        /// <param name="algorithmToCheck"></param>
        /// <returns></returns>
        protected bool CanBeParentOf(Boa_Algorithm algorithmToCheck)
        {
            if (!OutputIsFixedSize || !algorithmToCheck.InputIsFixedSize)
                return true;

            //at this point both algorithms are fixed
            if (OutputDimension >= algorithmToCheck.InputDimension)
                return true;

            return false;
        }

        /// <summary>
        /// Sets the input and output dimensions
        /// </summary>
        /// <param name="inputDimension"></param>
        /// <param name="outputDimension"></param>
        protected bool TrySetDimensions(int inputDimension, int outputDimension)
        {
            //if input is fixed
            if (inputDimension != 0)
            {
                if (HasParent)
                    if (!parent.TryMatchOutputDimension(inputDimension))
                        return false;

                InputIsFixedSize = true;
            }
            else
                InputIsFixedSize = false;

            //if output is fixed
            if (outputDimension != 0)
                OutputIsFixedSize = true;
            else
                OutputIsFixedSize = false;

            InputDimension = inputDimension;
            OutputDimension = outputDimension;

            return true;
        }

        protected bool TryMatchInputDimension (int outputToMatch)
        {
            //we don't need to match a non-fixed output
            if (outputToMatch == 0) return true;

            //if this input is fixed
            if (InputIsFixedSize)
            {
                if (InputDimension == outputToMatch) return true;
                else return false;
            }

            int newOutputDimension = OutputDimension;

            if (!OutputIsFixedSize)
                newOutputDimension = outputToMatch;

            return TrySetDimensions(outputToMatch, newOutputDimension);
        }

        protected bool TryMatchOutputDimension(int inputToMatch)
        {
            if (inputToMatch == 0) return true;

            //if this output is fixed
            if (OutputIsFixedSize)
            {
                if (OutputDimension == inputToMatch) return true; 
                else return false;
            }

            int newInputDimension = InputDimension;

            if (!InputIsFixedSize)
                newInputDimension = inputToMatch;

            return TrySetDimensions(newInputDimension, inputToMatch);
        }

        /// <summary>
        /// Solves the collection of algorithms, feeding the output of the first algorithm in the list to the input of the next one, and so on.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double[] Solve(double[] inputs)
        {
            SolutionRemark remark = 0;
            return Solve(inputs, out remark);
        }

        /// <summary>
        /// Same as Solve, but will relay if the solution was aborted due to incompatible dimensions.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="successful"></param>
        /// <returns></returns>
        public double[] Solve(double[] inputs, out SolutionRemark remark)
        {
            //double[] results = (double[])inputs.Clone();

            if (HasParent)
            {
                inputs = parent.Solve(inputs, out remark);
                if (remark.Equals(SolutionRemark.UndersizedInputs)) return inputs;
            }

            if (inputs.Length < InputDimension)
            {
                remark = SolutionRemark.UndersizedInputs;
                return inputs;
            }

            else if (InputIsFixedSize && inputs.Length > InputDimension)
                remark = SolutionRemark.OversizedInputs;
            else
                remark = SolutionRemark.Successful;

            return solveAlgorithm(inputs);
        }

        /// <summary>
        /// Solve but takes a Point3d, which is converted to a 3 item double array.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="successful"></param>
        /// <returns></returns>
        public double[] Solve(Point3d point, out SolutionRemark remark)
        {
            return Solve(new double[] { point.X, point.Y, point.Z }, out remark);
        }

        public bool IsValid
        {
            get
            {
                if (this.solveAlgorithm == null) return false;
                else return true;
            }
        }

        public string IsValidWhyNot => "Algorithm has no solution";

        public string TypeName => "Boa_Algorithm";

        public string TypeDescription => "A function that takes some number of inputs and returns some number of outputs";

        public bool CastFrom(object source)
        {
            return false;
            //throw new NotImplementedException();
        }

        public bool CastTo<T>(out T target)
        {
            target = default;
            return false;
            //throw new NotImplementedException();
        }

        public IGH_Goo Duplicate()
        {
            return new Boa_Algorithm(this);
        }

        public IGH_GooProxy EmitProxy()
        {
            throw new NotImplementedException();
        }

        public bool Read(GH_IReader reader)
        {
            throw new NotImplementedException();
        }

        public bool Write(GH_IWriter writer)
        {
            throw new NotImplementedException();
        }

        public object ScriptVariable()
        {
            throw new NotImplementedException();
        }

        public void AddDisjointedBase(Boa_Algorithm baseToAdd)
        {
            disjointedBase = baseToAdd;
            hasDisjointedBase = true;
        }

        public Boa_Algorithm GetBaseAlgorithm()
        {
            if (HasParent) return parent.GetBaseAlgorithm();
            if (hasDisjointedBase) return disjointedBase;
            return this;
        }

        public List<int> GetChainDimensions()
        {
            List<int> chainDimensions;

            if (HasParent)
            {
                chainDimensions = parent.GetChainDimensions();
            }

            else
            {
                chainDimensions = new List<int>();
                chainDimensions.Add(InputDimension);
            }

            int lastItemIndex = chainDimensions.Count - 1;

            if (chainDimensions[lastItemIndex] > InputDimension)
                chainDimensions[lastItemIndex] = InputDimension;
            //chainDimensions.Add(InputDimension);
            chainDimensions.Add(OutputDimension);

            return chainDimensions;
        }

        public string GetDimensionsString()
        {
            string dimensions = "N:N";

            if (InputDimension != 0)
                dimensions = InputDimension.ToString();
            else
                dimensions = "N";

            dimensions += ":";

            if (OutputDimension != 0)
                dimensions += OutputDimension.ToString();
            else
                dimensions += "N";

            return dimensions;
        }

        public string GetChainDimensionsString()
        {
            string algorithmDimensions = "Boa_Algorithm ";
            int currentDimension = InputDimension;

            List<int> chainDimensions = GetChainDimensions();

            for (int i = 0; i < chainDimensions.Count; i++)
            {
                string d;
                if (chainDimensions[i] == 0) d = "N";
                else d = chainDimensions[i].ToString();
                algorithmDimensions += d + ":";
            }

            return algorithmDimensions.Substring(0, algorithmDimensions.Length - 1);
        }

        public override string ToString()
        {
            return GetChainDimensionsString();
        }

        protected static double[] SolveAlgorithmDefault (double[] inputs)
        {
            return inputs;
        }
    }
}
