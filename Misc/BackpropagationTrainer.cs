using System;
using Accord.Neuro;
using Accord.Neuro.Learning;

namespace Boa
{
    public class BackpropagationTrainer : ISupervisedTrainer
    {
        private readonly int standard = 0;
        private readonly int resilient = 1;
        private readonly int parallelResilient = 2;

        private ISupervisedLearning teacher;

        public BackpropagationTrainer()
        {
        }

        public BackpropagationTrainer(NeuralNetwork network, int type, double learningRate)
        {
            if (type == resilient)
            {
                var rbpl = new ResilientBackpropagationLearning(network.ActivationNetwork);
                rbpl.LearningRate = learningRate;
                teacher = (ISupervisedLearning) rbpl;
            }
            //TODO: find a way to implement "learningRate" with PRBL network!
            else if (type == parallelResilient)
                teacher = new ParallelResilientBackpropagationLearning(network.ActivationNetwork);
            else
            {
                var bpl = new BackPropagationLearning(network.ActivationNetwork);
                bpl.LearningRate = learningRate;
                teacher = (ISupervisedLearning)bpl;
            }
        }

        ///<summary>
        /// Trains the neural network on input training data using backpropagation and returns the final error.
        ///</summary>
        public double Train(double[][] trainingData, double[][] targetResults)
        {
            return teacher.RunEpoch(trainingData, targetResults);
        }
    }
}
