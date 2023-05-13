using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DSUtilities.Section
{
    public static class Analysis
    {
        public static double tol = 1e-2;

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

        public static bool SolidVoidCheck(List<Curve> positives, List<Curve> negatives, int plane)
        {

            Plane worldplane = APtoWorld(plane);

            foreach (Curve negative in negatives)
            {

                bool check = false;

                foreach (Curve positive in positives)
                {
                    var res = (int)Curve.PlanarClosedCurveRelationship(negative, positive, worldplane, tol);

                    if (res == 1 || res == 3)
                    {
                        return false;
                    }

                    if (res == 2)
                    {
                        check = true;
                    }
                }
    
                if (!check)
                {
                    return false;
                }

            }

            return true;
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

        /// <summary>
        /// Get the bounding box of a collection of curves
        /// </summary>
        /// <param name="curves"></param>
        /// <returns></returns>
        public static BoundingBox GetBB(List<Curve> curves)
        {
            BoundingBox bb = curves[0].GetBoundingBox(true);

            if (curves.Count > 1)
            {
                for (int i = 1; i < curves.Count; i++)
                {
                    bb.Union(curves[i].GetBoundingBox(true));
                }
            }

            return bb;
        }

        /// <summary>
        /// Reference planes of analysis: first letter is the "Strong" axis (assumed horizontal)
        /// </summary>
        public enum AnalysisPlane
        {
            XY,
            YZ,
            XZ
        }

        public static Plane APtoWorld(int plane)
        {
            if (plane == 0)
            {
                return Plane.WorldXY;
            }
            else if (plane == 1)
            {
                return Plane.WorldYZ;
            }
            else
            {
                return Plane.WorldZX;
            }
        }


        public static int StrongDir(AnalysisPlane plane)
        {
            switch (plane)
            {
                case AnalysisPlane.XY:
                    return 1;
                case AnalysisPlane.YZ:
                    return 2;
                case AnalysisPlane. XZ:
                    return 2;
                default: return 1;
            }
        }

        public static int WeakDir(Analysis.AnalysisPlane plane)
        {
            switch (plane)
            {
                case Analysis.AnalysisPlane.XY:
                    return 0;
                case Analysis.AnalysisPlane.YZ:
                    return 1;
                case Analysis.AnalysisPlane.XZ:
                    return 0;
                default: return 0;
            }
        }

        public static List<Point3d> CornerPoints(AbstractSection section)
        {
            AnalysisPlane plane = section.Plane;
            BoundingBox bb = section.BoundingBox;

            switch(plane)
            {
                case AnalysisPlane.XY:
                    return new List<Point3d> {bb.Corner(true, false, true), bb.Corner(false, false, true), bb.Corner(true, true, true), bb.Corner(false, true, true)};
                case AnalysisPlane.YZ:
                    return new List<Point3d> {bb.Corner(true, true, false), bb.Corner(true, false, false), bb.Corner(true, true, true), bb.Corner(true, false, true) };
                case AnalysisPlane.XZ:
                    return new List<Point3d> { bb.Corner(true, true, false), bb.Corner(false, true, false), bb.Corner(true, true, true), bb.Corner(false, true, true) };
                default: return new List<Point3d> { bb.Corner(true, false, true), bb.Corner(false, false, true), bb.Corner(true, true, true), bb.Corner(false, true, true) };
            }
        }

        public static List<Point3d> CornerPoints(BoundingBox bb, AnalysisPlane plane)
        {
            switch (plane)
            {
                case AnalysisPlane.XY:
                    return new List<Point3d> { bb.Corner(true, false, true), bb.Corner(false, false, true), bb.Corner(true, true, true), bb.Corner(false, true, true) };
                case AnalysisPlane.YZ:
                    return new List<Point3d> { bb.Corner(true, true, false), bb.Corner(true, false, false), bb.Corner(true, true, true), bb.Corner(true, false, true) };
                case AnalysisPlane.XZ:
                    return new List<Point3d> { bb.Corner(true, true, false), bb.Corner(false, true, false), bb.Corner(true, true, true), bb.Corner(false, true, true) };
                default: return new List<Point3d> { bb.Corner(true, false, true), bb.Corner(false, false, true), bb.Corner(true, true, true), bb.Corner(false, true, true) };
            }
        }

        public static Vector3d UnitVector(AnalysisPlane plane)
        {
            switch (plane)
            {
                case AnalysisPlane.XY:
                    return new Vector3d(0, 1, 0);
                case AnalysisPlane.YZ:
                    return new Vector3d(0, 0, 1);
                case AnalysisPlane.XZ:
                    return new Vector3d(0, 0, 1);
                default:
                    return new Vector3d(0, 1, 0);
            }
        }

        public static Vector3d UnitVectorWidth(AnalysisPlane plane)
        {
            switch (plane)
            {
                case AnalysisPlane.XY:
                    return new Vector3d(1, 0, 0);
                case AnalysisPlane.YZ:
                    return new Vector3d(0, 1, 0);
                case AnalysisPlane.XZ:
                    return new Vector3d(1, 0, 0);
                default:
                    return new Vector3d(1, 0, 0);
            }
        }

        public static double GetArea(Section section, Curve intersector)
        {
            double area = 0;

            foreach (Curve curve in section.Solids)
            {
                Curve[] intersects = Curve.CreateBooleanIntersection(curve, intersector, tol);

                if (intersects.Length > 0)
                {
                    foreach (Curve inter in intersects)
                    {
                        area += AreaMassProperties.Compute(inter).Area;
                    }
                }
            }

            if (section.Voids.Count > 0)
            {
                foreach (Curve curve in section.Voids)
                {
                    Curve[] intersects = Curve.CreateBooleanIntersection(curve, intersector, Analysis.tol);

                    if (intersects.Length > 0)
                    {
                        foreach (Curve inter in intersects)
                        {
                            area -= AreaMassProperties.Compute(inter).Area;
                        }
                    }

                }
            }

            return area;
        }


        public static double OverlapArea(Section section, double depth, int direction)
        {
            Vector3d vec = UnitVector(section.Plane);
            Plane referenceplane = APtoWorld((int)section.Plane);
            Point3d startpoint;
            Point3d endpoint;
            //extract relevant corner points
            if (direction == -1)
            {
                startpoint = section.Corners[0];
                endpoint = section.Corners[1] - depth * vec;
            }
            else
            {
                startpoint = section.Corners[2];
                endpoint = section.Corners[3] + depth * vec;
            }

            Curve region = new Rectangle3d(referenceplane, startpoint, endpoint).ToNurbsCurve();

            return GetArea(section, region);

        }

        public static Point3d GetOverlapCentroid(Section section, Curve intersector)
        {
            double area = 0;
            Point3d centroid = new Point3d(0,0,0);

            foreach (Curve curve in section.Solids)
            {
                Curve[] intersects = Curve.CreateBooleanIntersection(curve, intersector, tol);

                if (intersects.Length > 0)
                {
                    foreach (Curve inter in intersects)
                    {
                        var amp = AreaMassProperties.Compute(inter);
                        area += amp.Area;
                        centroid += amp.Centroid * amp.Area;
                    }
                }
            }

            if (section.Voids.Count > 0)
            {
                foreach (Curve curve in section.Voids)
                {
                    Curve[] intersects = Curve.CreateBooleanIntersection(curve, intersector, Analysis.tol);

                    if (intersects.Length > 0)
                    {
                        foreach (Curve inter in intersects)
                        {
                            var amp = AreaMassProperties.Compute(inter);
                            area -= amp.Area;
                            centroid += -amp.Centroid * amp.Area;
                        }
                    }

                }
            }

            return centroid / area;
        }

    }
}