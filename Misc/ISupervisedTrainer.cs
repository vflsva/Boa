using System;
namespace Boa
{
    public interface ISupervisedTrainer
    {
        double Train(double[][] trainingData, double[][] targetResults);
    }
}
