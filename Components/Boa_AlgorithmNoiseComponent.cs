using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using LibNoise;
using LibNoise.Primitive;

namespace Boa
{
    public abstract class Boa_Algorithm3DNoiseComponent : Boa_AlgorithmComponent
    {
        protected LibNoise.IModule3D noise;

        public Boa_Algorithm3DNoiseComponent(string name, string nickname, string description)
          : base(name, nickname,
            description,
            "Boa", "Noise")
        {
        }

        public Boa_Algorithm3DNoiseComponent(string noiseType)
          : this(noiseType + " Noise", noiseType,
            "3 dimensional " + noiseType + " noise algorithm.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            AddSeedInput(pManager);
            AddNoiseQualityInput(pManager);
        }

        protected void AddSeedInput(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Seed", "S", "Seed to use to generate noise", GH_ParamAccess.item, 0);
        }

        protected void AddNoiseQualityInput(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Quality", "Q", "Quality of the noise. 0 = Standard, 1 = Fast, 2 = Best", GH_ParamAccess.item, 0);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int seed = 0;
            NoiseQuality noiseQuality = NoiseQuality.Standard;

            if (!GetSeedInput(DA, ref seed)) return;
            if (!GetNoiseQualityInput(DA, ref noiseQuality)) return;

            CreateNoise(DA, seed, noiseQuality);

            CreateAndSetAlgorithm(DA, SolveNoise, 3, 1);
        }

        protected virtual void CreateNoise(IGH_DataAccess DA, int seed, NoiseQuality noiseQuality)
        {
            noise = new SimplexPerlin(seed, noiseQuality);
        }

        public double[] SolveNoise(double[] inputs)
        {
            return new double[] { noise.GetValue((float)inputs[0], (float)inputs[1], (float)inputs[2]) };
        }

        protected bool GetSeedInput(IGH_DataAccess DA, ref int seed)
        {
            if (!DA.GetData(0, ref seed))
                return false;

            return true;
        }

        protected bool GetNoiseQualityInput(IGH_DataAccess DA, ref NoiseQuality noiseQuality)
        {
            int q = 0;

            if (!DA.GetData(1, ref q))
                return false;

            switch (q)
            {
                case 0:
                    noiseQuality = NoiseQuality.Standard;
                    break;
                case 1:
                    noiseQuality = NoiseQuality.Fast;
                    break;
                case 2:
                    noiseQuality = NoiseQuality.Best;
                    break;
                default:
                    noiseQuality = NoiseQuality.Standard;
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Quality must be between 0 and 2.");
                    break;
            }

            return true;
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d8d23ae0-9c7d-45f7-b086-2e9362ddc196"); }
        }
    }
}
