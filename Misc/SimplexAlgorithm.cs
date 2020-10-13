using System;
using LibNoise;
namespace Boa
{
    public class SimplexAlgorithm : LibNoiseWrapper
    {
        public SimplexAlgorithm(int seed, int quality) : base(seed, quality)
        {
            noise = new LibNoise.Primitive.SimplexPerlin(seed, noiseQuality);
        }

        public SimplexAlgorithm()
        {
            noise = new LibNoise.Primitive.SimplexPerlin();
        }
    }
}
