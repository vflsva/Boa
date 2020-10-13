using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using LibNoise;
using LibNoise.Primitive;

namespace Boa
{
    public class PerlinAlgorithmComponent : Boa_Algorithm3DNoiseComponent
    {

        public PerlinAlgorithmComponent()
          : base("Perlin")
        {
        }

        protected override void CreateNoise(IGH_DataAccess DA, int seed, NoiseQuality noiseQuality)
        {
            noise = new ImprovedPerlin(seed, noiseQuality);
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.perlinNoiseIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("8f6f06cb-a543-435f-b9d6-a57d78937eae"); }
        }
    }
}
