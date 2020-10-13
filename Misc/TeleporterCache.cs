using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Boa
{
    public static class TeleporterCache
    {
        //private static Type cache = typeof(List<>);
        private static List<GH_Structure<IGH_Goo>> cache = new List<GH_Structure<IGH_Goo>>();
        private static List<GH_Component> downstreamComponents = new List<GH_Component>();


        public static int Count => cache.Count;


        public static GH_Structure<IGH_Goo> GetItem(int i) { return cache[i]; }

        public static void SetItem(GH_Structure<IGH_Goo> item, int i)
        {
            //index must be greater than or equal to 0
            if (i < 0) return;

            //if index is out of bounds, then simply add empty GH_Structures until our list is big enough.
            while (cache.Count <= i)
            {
                cache.Add(new GH_Structure<IGH_Goo>());
            }

            cache[i] = item;

            //refresh all downstream components
            foreach (GH_Component c in downstreamComponents)
            {
                c.ExpireSolution(true);
                //c.ClearData();
                c.CollectData();
                c.ComputeData();
            }
        }

        public static void AddDownstreamComponent(GH_Component component)
        {
            downstreamComponents.Add(component);
        }
    }
}
