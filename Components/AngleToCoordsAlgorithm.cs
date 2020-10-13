using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Boa
{
    public class AngleToCoordsAlgorithm : Boa_AlgorithmComponent
    {
        private Plane basePlane;

        public AngleToCoordsAlgorithm()
          : base("Angle to Coords Algorithm", "Angle2XYZ",
            "Algorithm that returns unit coordinates for a given angle (radians) in the plane.")
        {
        }

        protected override void RegisterAdditionalInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "Plane", "Plane on which the angle should be mapped to", GH_ParamAccess.item, Plane.WorldXY);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData(0, ref basePlane)) return;

            var angleToCoords = new AngleToCoords(basePlane);
            CreateAndSetAlgorithm(DA, angleToCoords.SolveAlgorithm, 1, 3);
        }

        protected override System.Drawing.Bitmap Icon => Boa2.Properties.Resource1.angleToCoordsAlgorithmIcon;

        public override Guid ComponentGuid
        {
            get { return new Guid("f6b06721-152a-49f1-a086-8b9b6e9cc0f4"); }
        }
    }

    public class AngleToCoords :IBoa_AlgorithmSolution
    {
        private Plane basePlane;

        public AngleToCoords(Plane basePlane)
        {
            this.basePlane = basePlane;
        }


        public double[] SolveAlgorithm(double[] inputs)
        {
            Point3d point = (Point3d)basePlane.XAxis * Math.Cos(inputs[0]) + basePlane.YAxis * Math.Sin(inputs[0]);
            return new double[] { point.X, point.Y, point.Z };
        }
    }
}
