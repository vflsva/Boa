using System;
using LibNoise;

namespace Boa
{
    public abstract class LibNoiseWrapper : IBoa_AlgorithmSolution
    {
        //private readonly int dimensions;

        protected LibNoise.IModule3D noise;
        protected NoiseQuality noiseQuality;

        public LibNoiseWrapper(int seed, int quality)
        {
            //this.dimensions = dimensions;

            switch (quality)
            {
                case 1:
                    noiseQuality = NoiseQuality.Fast;
                    break;
                case 2:
                    noiseQuality = NoiseQuality.Best;
                    break;
                default:
                    noiseQuality = NoiseQuality.Standard;
                    break;
            }
        }

        public LibNoiseWrapper() : this(0, 0)
        { }

        public double[] SolveAlgorithm(double[] inputs)
        {
            float x = 0, y = 0, z = 0;

            if (inputs.Length == 1)
            {
                x = (float)inputs[0];
            }
            else if (inputs.Length == 2)
            {
                x = (float)inputs[0];
                y = (float)inputs[1];
            }
            else if (inputs.Length >= 3)
            {
                x = (float)inputs[0];
                y = (float)inputs[1];
                z = (float)inputs[2];
            }

            float result = noise.GetValue(x, y, z);
            return new double[] { (double)result };
        }
    }
}
