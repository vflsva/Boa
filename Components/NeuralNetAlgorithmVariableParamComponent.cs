using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace Boa
{
    public class NeuralNetAlgorithmVariableParamComponent : NeuralNetAlgorithmComponent, IGH_VariableParameterComponent
    {
        private readonly string hiddenLayerParamDescription = "Number of neurons in hidden layer ";
        private readonly int hiddenLayerStartIndex = 1;
        private int hiddenLayerEndIndex { get { return Params.Input.Count - 6; } }
        private int nHiddenLayers { get { return hiddenLayerEndIndex - hiddenLayerStartIndex + 1; } }

        public NeuralNetAlgorithmVariableParamComponent()
          : base("Neural Network Variable Param", "NeuralNet",
            "Neural Network Algorithm with variable layer parameters for convenience.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("nInputs", "I", "Number of inputs to the network", GH_ParamAccess.item);
            pManager.AddIntegerParameter("nHidden", "H", hiddenLayerParamDescription + "1", GH_ParamAccess.item);
            pManager.AddIntegerParameter("nOutputs", "O", "Number of outputs from the network", GH_ParamAccess.item);
            AddActivationParam(pManager);
            AddAlphaParam(pManager);
            AddResetParam(pManager);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //nInputs   i = 0
            //h1        i = 1
            //h2        i = 2
            //h3        i = 3
            //nOutputs  i = 4
            //actType   i = 5
            //reset     i = 6
            //alpha     i = 7
            //parent    i = 8, Count = 9

            int[] layers = new int[nHiddenLayers + 2];// new List<int>();

            int activationType = 0;
            double alpha = 0;
            bool reset = false;

            //getting input size
            if (!DA.GetData(0, ref layers[0])) return;
            //getting hidden layer sizes
            for (int i = hiddenLayerStartIndex; i <= hiddenLayerEndIndex; i++)
                if (!DA.GetData(i, ref layers[i])) return;
            //getting output layer size
            if (!DA.GetData(hiddenLayerEndIndex + 1, ref layers[layers.Length - 1])) return;

            if (!DA.GetData(hiddenLayerEndIndex + 2, ref activationType)) return;
            if (!DA.GetData(hiddenLayerEndIndex + 3, ref alpha)) return;
            DA.GetData(hiddenLayerEndIndex + 4, ref reset);

            if (!CheckReset(reset, DA)) return;
            if (!ValidateLayers(layers)) return;

            int[] numberOfNeurons = new int[layers.Length - 1];
            for (int i = 0; i < numberOfNeurons.Length; i++)
                numberOfNeurons[i] = layers[i + 1];

            bool successful = false;
            var function = CreateActivationFunction(activationType, alpha, out successful);

            if (!successful) return;

            Network = new NeuralNetwork(function, layers[0], numberOfNeurons);

            CreateAndSetAlgorithm(DA, Network.SolveAlgorithm, Network.InputSize, Network.OutputSize);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Output) return false;
            //we do not want our first or last three parameters to be variable
            if (index == 0 || index >= Params.Input.Count - 4) return false;
            return true;

        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Output) return false;
            //we do not want to remove our first or last three parameters
            if (index == 0 || index >= Params.Input.Count - 5) return false;
            if (Params.Input.Count <= 7) return false;
            return true;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            IGH_Param p = new Param_Integer();
            p.Name = "nHidden";// + index.ToString();
            p.NickName = "H";
            p.Description = hiddenLayerParamDescription;
            return p;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            int index = 0;
            for (int i = hiddenLayerStartIndex; i <= hiddenLayerEndIndex; i++)
            {
                index ++;
                Params.Input[i].Description = hiddenLayerParamDescription + index.ToString();
            }
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.neuralNetworkIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("ff0595cd-0897-49dd-98ac-cc660402ff48"); }
        }
    }
}
