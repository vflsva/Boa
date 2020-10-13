using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using LibNoise.Primitive;
using LibNoise;

namespace Boa
{
    public class SimplexAlgorithmComponent : Boa_Algorithm3DNoiseComponent
    {
        public SimplexAlgorithmComponent()
          : base("Simplex")
        {
        }

        protected override void CreateNoise(IGH_DataAccess DA, int seed, NoiseQuality noiseQuality)
        {
            noise = new SimplexPerlin(seed, noiseQuality);
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.simplexNoiseIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("f5513421-34d6-4fa6-b9ed-99e1c9808bd2"); }
        }
    }
}
