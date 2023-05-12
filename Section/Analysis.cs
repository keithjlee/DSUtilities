using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Section
{
    internal static class Analysis
    {
        /// <summary>
        /// Checks if sections are closed and planar
        /// </summary>
        /// <param name="curves"></param>
        /// <returns></returns>
        public static bool ValidCheck(List<Curve> curves)
        {
            for (int i = 0; i < curves.Count; i++)
            {
                Curve curve = curves[i];
                if (curve.IsClosed && curve.IsPlanar())
                {
                    continue;
                }
                else
                {
                    return false; //if any of the curves are open or non-planar, return false
                }
            }

            return true; //else return true
        }

        /// <summary>
        /// Computes the total area of an arbitrary section
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static double TotalArea(List<Curve> sections)
        {
            double Atotal = 0.0;
            for (int i = 0; i < sections.Count; i++)
            {
                Curve curve = sections[i];
                var curveProp = AreaMassProperties.Compute(curve);
                Atotal += curveProp.Area;
            }

            return Atotal;
        }

        public static BoundingBox GetBB(List<Curve> curves)
        {
            BoundingBox bb = new BoundingBox();

            for (int i = 0; i <= curves.Count; i++)
            {
                bb.Union(curves[i].GetBoundingBox(true));
            }

            return bb;
        }

        public static List<double> SectionModulii(Point3d Centroid, BoundingBox bb)
        {
            List<double> S = new List<double>();
            // X axis bending
            var vxtop = bb.Corner(true, false, false) - Centroid;
            var dxtop = Math.Sqrt(Math.Pow(vxtop.Y, 2) + Math.Pow(vxtop.Z, 2));
            S.Add(dxtop);

            var vxbot = bb.Corner(true, false, true) - Centroid;
            var dxbot = Math.Sqrt(Math.Pow(vxbot.Y, 2) + Math.Pow(vxbot.Z, 2));
            S.Add(dxbot);
        }
    }
}
