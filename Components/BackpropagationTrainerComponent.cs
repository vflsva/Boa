using System;
using System.Windows.Forms;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using System.Drawing;
using System.Drawing.Drawing2D;
using static Boa.NeuralNetAlgorithmComponent;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace Boa
{
    public class BackpropagationTrainerComponent : Boa_TrainerComponent
    {
        private readonly string[] backpropagationTypes = { "Standard", "Resilient", "Parallel Resilient" };
        //private BackpropagationTrainer trainer;

        //private NeuralNetwork neuralNet;
        private double error = 0;

        public BackpropagationTrainerComponent()
          : base("Backpropagation Trainer", "Backprop",
            "Trains a neural network using backpropagation.")
        {
            
            //HasBeenLinked = false;
        }

        public override void CreateAttributes()
        {
            m_attributes = new NeuralNetTrainerComponentAttributes(this);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            Param_GenericObject param = new Param_GenericObject();
            param.Name = "Neural Network";
            param.NickName = "Network";
            param.Description = "Neural Network to train";
            param.Access = GH_ParamAccess.item;
            param.Locked = true;
            param.Hidden = true;
            param.Optional = true;
            param.Attributes = new NeuralNetTrainerParamAttributes(param, m_attributes);
            //param.Attributes.H
            pManager.AddParameter(param);

            pManager.AddNumberParameter("Training Data", "Data", "Input values to train network on", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Target Values", "Target", "Known output from training data", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Backpropagation Type", "Type", "Type of backpropagation. " + BackpropagationTypesDescription(), GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Learning Rate", "Rate", "Amount to adjust weights within each iteration.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Epochs", "Epochs", "Number of times to cycle through the training data", GH_ParamAccess.item, 100);
            pManager.AddBooleanParameter("Train", "Train", "Run another round of training. Use button to toggle.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Error", "E", "Average error from last epoch", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<GH_Number> trainingDataTree = new GH_Structure<GH_Number>();
            GH_Structure<GH_Number> targetValuesTree = new GH_Structure<GH_Number>();
            int type = 0;
            double learningRate = 0;
            int epochs = 0;
            bool reset = false;

            //if (!HasBeenLinked)
            //{
            //    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No neural network has been selected for training");
            //    return;
            //}
            //else
            //    neuralNet = neuralNetComponent.Network;

            //SetNetworkToParam();
            bool successful;
            var neuralNet = GetNetwork(out successful);// new NeuralNetwork();
            if (!successful)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Trainer has not been linked to a neural network.");
                return;
            }

            //if (!DA.GetData(0, ref neuralNet)) return;
            if (!DA.GetDataTree(1, out trainingDataTree)) return;
            if (!DA.GetDataTree(2, out targetValuesTree)) return;
            if (!DA.GetData(3, ref type)) return;
            if (!DA.GetData(4, ref learningRate)) return;
            if (!DA.GetData(5, ref epochs)) return;

            DA.GetData(6, ref reset);

            if (type < 0 || type >= backpropagationTypes.Length)
            {
                AddRuntimeMessage(
                    GH_RuntimeMessageLevel.Error,
                    "Backpropagation Type must be between 0 and " + (backpropagationTypes.Length - 1).ToString() + "."
                    );
                return;
            }
            if (epochs < 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Number of epochs must be 1 or greater.");
                return;
            }
            if (learningRate < 0 || learningRate > 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Learning rate must be between 0 and 1.");
                return;
            }

            //if we DID NOT update/reset the network
            if (!reset)
            {
                DA.SetData(0, error);
                return;
            }

            var trainingDataBranches = trainingDataTree.Branches;
            int numBranches = trainingDataBranches.Count;
            int itemsInBranch = trainingDataBranches[0].Count;
            double[][] trainingData2D = new double[numBranches][];

            for (int i = 0; i < numBranches; i++)
            {
                trainingData2D[i] = new double[itemsInBranch];

                //copying values from the data tree into the array
                for (int j = 0; j < itemsInBranch; j++)
                    trainingData2D[i][j] = trainingDataBranches[i][j].Value;

                //if the neural net has a parent algorithm, then we'll run the training data through it first
                if (neuralNetComponent.Algorithm.HasParent)
                    trainingData2D[i] = neuralNetComponent.Algorithm.Parent.Solve(trainingData2D[i]);
            }

            //checking to see if our training data is the right size.
            if (trainingData2D[0].Length != neuralNet.InputSize)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Training data must match the size of the network's inputs.");
            }

            var targetValuesBranches = targetValuesTree.Branches;
            numBranches = targetValuesBranches.Count;
            itemsInBranch = targetValuesBranches[0].Count;
            double[][] targetValues2D = new double[numBranches][];

            for (int i = 0; i < numBranches; i++)
            {
                targetValues2D[i] = new double[itemsInBranch];

                for (int j = 0; j < itemsInBranch; j++)
                    targetValues2D[i][j] = targetValuesBranches[i][j].Value;
            }

            //checking to see if our target values are the right size.
            if (targetValues2D[0].Length != neuralNet.OutputSize)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Target values must match the size of the network's outputs.");
            }

            BackpropagationTrainer trainer = new BackpropagationTrainer(neuralNet, type, learningRate);
            for (int i = 0; i < epochs; i++)
                error = trainer.Train(trainingData2D, targetValues2D) / numBranches;

            //neuralNetComponent.Network = neuralNet;
            //neuralNetComponent.CollectData();
            //neuralNetComponent.ComputeData();
            neuralNetComponent.ExpireSolution(true);
            //DA.SetData(0, new Boa_Algorithm(neuralNet.SolveAlgorithm, neuralNet.InputSize, neuralNet.OutputSize));
            DA.SetData(0, error);
        }

        //public void Train(int epochs)
        //{
        //    BackpropagationTrainer trainer = new BackpropagationTrainer(neuralNet, type, learningRate);
        //    for (int i = 0; i < epochs; i++)
        //        error = trainer.Train(trainingData2D, targetValues2D) / numBranches;
        //}


        /// <summary>
        /// Returns a string equating index positions to their corresponding activation functions.
        /// ex. "0 = Bipolar Sigmoid, 1 = ..."
        /// </summary>
        /// <returns></returns>
        string BackpropagationTypesDescription()
        {
            string backpropagationTypesDescription = "";

            for (int i = 0; i < backpropagationTypes.Length; i++)
            {
                if (i != 0) backpropagationTypesDescription += ", ";
                backpropagationTypesDescription += i.ToString() + " = " + backpropagationTypes[i];
            }
            return backpropagationTypesDescription;
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.backpropagationTrainerIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("037c9ced-dab3-4efc-a175-6bd628151237"); }
        }
    }
}
