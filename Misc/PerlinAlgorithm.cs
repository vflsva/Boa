using System;
using LibNoise;

namespace Boa
{
    public class PerlinAlgorithm : LibNoiseWrapper
    {
        public PerlinAlgorithm(int seed, int quality) : base(seed, quality)
        {
            noise = new LibNoise.Primitive.ImprovedPerlin(seed, noiseQuality);
        }

        public PerlinAlgorithm()
        {
            noise = new LibNoise.Primitive.ImprovedPerlin();
        }
    }
}
