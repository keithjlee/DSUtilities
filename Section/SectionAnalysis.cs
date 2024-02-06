using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Section
{
    internal static class SectionAnalysis
    {

        /// <summary>
        /// Split a section with a horizontal line at depth y from centroid and return two lists of upper breps and lower breps
        /// </summary>
        /// <param name="section"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static List<List<Brep>> HorizontalSplit(Section section, double y)
        {
            // Make splitter brep
            Plane split_plane = new Plane(section.Plane.Origin, section.Plane.YAxis);

            // offset
            split_plane.OriginY += y;

            // make splitting surface
            Interval split_interval = new Interval(section.Xmin, section.Xmax);

            // turn into brep
            Brep splitter = new PlaneSurface(split_plane, split_interval, split_interval).ToBrep();

            // split brep
            Brep[] split_breps = section.Geo.Split(splitter, 1e-6);

            // storage lists for upper/lower
            List<Brep> upper = new List<Brep>();
            List<Brep> lower = new List<Brep>();

            // sort
            foreach (Brep brep in split_breps)
            {
                Point3d brep_centroid = AreaMassProperties.Compute(brep).Centroid;

                if (brep_centroid.Y > section.Centroid.Y)
                {
                    upper.Add(brep);
                }
                else
                {
                    lower.Add(brep);
                }
            }

            return new List<List<Brep>> { upper, lower };
        }

        /// <summary>
        /// Split a section with a vertical line at position x from the centroid and return two lists of left breps and right breps
        /// </summary>
        /// <param name="section"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static List<List<Brep>> VerticalSplit(Section section, double x)
        {
            // Make splitter brep
            Plane split_plane = new Plane(section.Plane.Origin, section.Plane.XAxis);

            // offset
            split_plane.OriginX += x;

            // make splitting surface
            Interval split_interval = new Interval(section.Ymin, section.Ymax);

            // turn into brep
            Brep splitter = new PlaneSurface(split_plane, split_interval, split_interval).ToBrep();

            // split brep
            Brep[] split_breps = section.Geo.Split(splitter, 1e-6);

            // storage lists for upper/lower
            List<Brep> right = new List<Brep>();
            List<Brep> left = new List<Brep>();

            // sort
            foreach (Brep brep in split_breps)
            {
                Point3d brep_centroid = AreaMassProperties.Compute(brep).Centroid;

                if (brep_centroid.X > section.Centroid.X)
                {
                    right.Add(brep);
                }
                else
                {
                    left.Add(brep);
                }
            }

            return new List<List<Brep>> { right, left};
        }

        /// <summary>
        /// Get the area-weighed centroid from a list of sections
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static Point3d CompoundCentroid(List<Section> sections)
        {
            Point3d centroid = new Point3d(0, 0, 0);
            double cumulative_area = 0;

            foreach (Section section in sections)
            {
                centroid += section.Centroid * section.Area;
                cumulative_area += section.Area;
            }

            return centroid / cumulative_area;
        }

        /// <summary>
        /// Get the compound moment of inertia from a list of sections
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static double[] ParallelAxisMomentOfInertia(List<Section> sections)
        {
            Vector3d compound_inertia = new Vector3d(0, 0, 0);
            Point3d centroid = CompoundCentroid(sections);

            foreach (Section section in sections)
            {

                Vector3d diff = section.Centroid - centroid;
                Vector3d factor = new Vector3d(Math.Pow(diff.Y, 2), Math.Pow(diff.X, 2), 0);

                compound_inertia += section.Properties.CentroidCoordinatesMomentsOfInertia + section.Area * factor;
            }

            return new double[] { compound_inertia[0], compound_inertia[1] };
        }
    }
}
