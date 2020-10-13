using Accord.Neuro.Learning;

namespace Boa
{
    public class EvolutionaryTrainer
    {
        private EvolutionaryLearning teacher;

        public EvolutionaryTrainer(NeuralNetwork neuralNetwork, int populationSize)
        {
            teacher = new EvolutionaryLearning(neuralNetwork.ActivationNetwork, populationSize);
        }

        ///<summary>
        /// Trains the neural network on input training data using an evolutionary algorithm and returns the final error.
        ///</summary>
        public double Train(double[][] trainingData, double[][] targetResults)
        {
            return teacher.RunEpoch(trainingData, targetResults);
        }
    }
}
