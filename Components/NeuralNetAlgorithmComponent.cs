using System;
using System.Drawing;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.GUI;
using Grasshopper.Kernel.Attributes;
using Rhino.Geometry;
using Accord.Neuro;
using Grasshopper.GUI.Canvas;

namespace Boa
{
    public class NeuralNetAlgorithmComponent : Boa_AlgorithmComponent
    {
        public NeuralNetwork Network { get; set; }
        private readonly string[] activationFunctions = { "Bipolar Sigmoid", "Sigmoid", "Rectified Linear", "Linear", "Identity", "Threshold" };
        /*to toggle when we reset the neural net. We don't want resetting the network to be too easy
         * because making any changes will mean re-initiating our weights and we will have to retrain
         * the network from scratch.
        */
        private bool lastReset = false;
        private bool forceReset = false;

        public NeuralNetAlgorithmComponent()
          : base("Neural Network", "NeuralNetwork",
            "Neural Network Algorithm.",
            "Boa", "Machine Learning")
        {
            NewInstanceGuid();
        }

        public NeuralNetAlgorithmComponent(string name, string nickname, string description)
          : base(name, nickname,
            description,
            "Boa", "Machine Learning")
        {
            NewInstanceGuid();
        }

        public override void CreateAttributes()
        {
            m_attributes = new NeuralNetAttributes(this);
        }

        public class NeuralNetAttributes : Boa_AlgrithmComponentAttributes//<NeuralNetAlgorithmComponent>
        {
            public PointF WireEnd { get { return new PointF(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height); } }

            public NeuralNetAttributes(NeuralNetAlgorithmComponent owner) : base(owner)
            {
            }

            protected override void Layout()
            {
                base.Layout();
            }

            //protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            //{
            //    if (channel.Equals(GH_CanvasChannel.Wires))
            //        WireEnd = new PointF(Bounds.X + Bounds.Width/2, Bounds.Y + Bounds.Height);

            //    base.Render(canvas, graphics, channel);
            //}
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Layers", "L", "Neural network layers as a list of integers.", GH_ParamAccess.list);
            AddActivationParam(pManager);
            AddAlphaParam(pManager);
            AddResetParam(pManager);
        }

        protected void AddActivationParam(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Activation", "F", "Activation Function. " + ActivationFunctionDescription(), GH_ParamAccess.item, 0);
        }

        protected void AddAlphaParam(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Alpha", "A", "Alpha value of Activation Function", GH_ParamAccess.item, 2.0);
        }

        protected void AddResetParam(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reset", "R", "Resets the Neural Network. WARNING: Doing this will erase all training data! Use Button to toggle", GH_ParamAccess.item, false);
        }

        //private 

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<int> layers = new List<int>();
            int activationType = 0;
            double alpha = 0;
            bool reset = false;

            if (!DA.GetDataList(0, layers)) return;
            if (!DA.GetData(1, ref activationType)) return;
            if (!DA.GetData(2, ref alpha)) return;
            DA.GetData(3, ref reset);

            if (!CheckReset(reset, DA)) return;
            if (!ValidateLayers(layers.ToArray())) return;

            int[] numberOfNeurons = new int[layers.Count - 1];
            layers.CopyTo(1, numberOfNeurons, 0, numberOfNeurons.Length);

            bool successful = false;
            var function = CreateActivationFunction(activationType, alpha, out successful);

            if (!successful) return;

            Network = new NeuralNetwork(function, layers[0], numberOfNeurons);

            CreateAndSetAlgorithm(DA, Network.SolveAlgorithm, Network.InputSize, Network.OutputSize);
        }

        public void ResetNetwork()
        {
            forceReset = true;
            ExpireSolution(true);
            forceReset = false;
        }

        protected bool CheckReset(bool reset, IGH_DataAccess DA)
        {
            //if we DID NOT update/reset the network
            if (!reset && !forceReset)
            {
                if (Network != null) CreateAndSetAlgorithm(DA, Network.SolveAlgorithm, Network.InputSize, Network.OutputSize);
                return false;
            }
            return true;
        }

        protected bool ValidateLayers(int[] layers)
        {
            if (layers.Length < 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Network must have at least 3 layers (Input, Hidden, Output)");
                return false;
            }

            if (layers[0] < 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Number of inputs must be greater than or equal to one.");
                return false;
            }

            for (int i = 1; i < layers.Length - 1; i++)
            {
                if (layers[i] < 1)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Size of hidden layer(s) must be greater than or equal to one.");
                    return false;
                }
            }

            if (layers[layers.Length - 1] < 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Number of outputs must be greater than or equal to one.");
                return false;
            }

            return true;
        }

        protected Accord.Neuro.IActivationFunction CreateActivationFunction(int activationType, double alpha, out bool successful)
        {
            //TODO: Figure out what "alpha" means in the constructor for these IActivationFunctions
            switch (activationType)
            {
                case 0:
                    successful = true;
                    return new Accord.Neuro.BipolarSigmoidFunction(alpha);

                case 1:
                    successful = true;
                    return new Accord.Neuro.SigmoidFunction(alpha);
                case 2:
                    successful = true;
                    return new Accord.Neuro.RectifiedLinearFunction();
                case 3:
                    successful = true;
                    return new Accord.Neuro.LinearFunction(alpha);
                case 4:
                    successful = true;
                    return new Accord.Neuro.IdentityFunction();
                case 5:
                    successful = true;
                    return new Accord.Neuro.ThresholdFunction();
            }

            successful = false;
            AddRuntimeMessage(
                GH_RuntimeMessageLevel.Error,
                "Activation type must be between 0 and " + (activationFunctions.Length - 1).ToString() + "."
                );
            return new Accord.Neuro.BipolarSigmoidFunction(alpha);
        }

        /// <summary>
        /// Returns a string equating index positions to their corresponding activation functions.
        /// ex. "0 = Bipolar Sigmoid, 1 = ..."
        /// </summary>
        /// <returns></returns>
        private string ActivationFunctionDescription()
        {
            string activationFunctionDescription = "";

            for (int i = 0; i < activationFunctions.Length; i++)
            {
                if (i != 0) activationFunctionDescription += ", ";
                activationFunctionDescription += i.ToString() + " = " + activationFunctions[i];
            }
            return activationFunctionDescription;
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.neuralNetworkIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("c1d355d9-3748-4aff-a182-b79381b71f8e"); }
        }
    }
}
