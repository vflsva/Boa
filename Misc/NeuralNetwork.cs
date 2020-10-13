using System;
using Accord.Neuro;
using Accord.Neuro.ActivationFunctions;
using Accord.Neuro.Learning;

namespace Boa
{
    public class NeuralNetwork : IBoa_AlgorithmSolution
    {
        private ActivationNetwork network;
        private Accord.Neuro.IActivationFunction activationFunction;
        private readonly int[] numberOfNeurons;
        private readonly int inputSize, outputSize;

        public int InputSize => inputSize;
        public int OutputSize => outputSize;

        public ActivationNetwork ActivationNetwork => network;

        public NeuralNetwork()
        {
            network = new ActivationNetwork(new BipolarSigmoidFunction(2), 1, 1);
            inputSize = 1;
            outputSize = 1;
            //InitalizeInputs(1);
            //InitalizeOutputs(1);
        }

        public NeuralNetwork(Accord.Neuro.IActivationFunction function, int numberOfInputs, params int[] numberOfNeurons)
        {
            inputSize = numberOfInputs;
            outputSize = numberOfNeurons[numberOfNeurons.Length - 1];

            activationFunction = function;
            this.numberOfNeurons = numberOfNeurons;
            network = new ActivationNetwork(function, numberOfInputs, numberOfNeurons);
            //InitalizeInputs(numberOfInputs);
            //InitalizeOutputs(numberOfNeurons[numberOfNeurons.Length - 1]);
        }

        //public double Train()
        //public override double[] Solve()//params double[] inputs)
        //{
        //    return network.Compute(inputs);
        //}

        public double[] SolveAlgorithm(double[] inputs)
        {
            return network.Compute(inputs);
        }

        //public override string GetName() { return "Neural Network"; }

        //public override IFunctionND Copy()
        //{
        //    return new NeuralNetwork(activationFunction, inputs.Length, numberOfNeurons);
        //}
    }
}
