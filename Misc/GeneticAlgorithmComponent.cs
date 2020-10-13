using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Accord.Genetic;

namespace Boa
{
    public class GeneticAlgorithmComponent : GH_Component
    {
        private Population population;


        public GeneticAlgorithmComponent()
          : base("Genetic Algorithm", "GeAlgo",
            "GeneticAlgorithmComponent description",
            "Boa", "Garbage")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int populationSize = 50;
            int chromosomeLength = 10;
            IChromosome ancestor = new BinaryChromosome(chromosomeLength);
            //population = new Population(populationSize, ancestor, )
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid
        {
            get { return new Guid("258d0439-4fe0-4295-85d6-439c209b928c"); }
        }
    }
}
