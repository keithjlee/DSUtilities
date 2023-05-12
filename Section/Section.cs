using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using MathNet.Numerics;

namespace DSUtilities.Section
{
    internal abstract class AbstractSection
    {
        public double Area;
        public Point3d Centroid;
        public double Istrong;
        public double Iweak;
        public Analysis.AnalysisPlane Plane;
        public BoundingBox BoundingBox;
    }
    internal class Section: AbstractSection
    {
        public double E = 1;
        public List<Curve> Solids;
        public List<Curve> Voids;
        public List<Curve> Geometry;

        public Section()
        {

        }

        public Section(List<Curve> curves, List<Curve> voids, double E, Analysis.AnalysisPlane plane)
        {
            //initialize
            Area = 0;
            Centroid = new Point3d();
            Solids = curves;
            Voids = voids;
            Plane = plane;
            this.E = E;

            //individual properties
            var areas = new List<double>();
            var centroids = new List<Point3d>();
            var inertias = new List<Vector3d>();

            //populate individual collectors
            foreach (Curve curve in curves)
            {
                var properties = AreaMassProperties.Compute(curve);

                areas.Add(properties.Area);
                Area += properties.Area;

                centroids.Add(properties.Centroid);
                Centroid += properties.Area * properties.Centroid;

                inertias.Add(properties.CentroidCoordinatesMomentsOfInertia);
            }

            if (voids.Count > 0)
            {
                foreach (Curve curve in voids)
                {
                    var properties = AreaMassProperties.Compute(curve);

                    areas.Add(-properties.Area);
                    Area -= properties.Area;

                    centroids.Add(properties.Centroid);
                    Centroid += -properties.Area * properties.Centroid;

                    inertias.Add(properties.CentroidCoordinatesMomentsOfInertia);
                }

                
            }

            //get total centroid
            Centroid /= Area;

            //get strong and weak moments of inertia
            GetInertias(centroids, inertias, areas);

            //get bounding box
            this.BoundingBox = Analysis.GetBB(curves);

            //get final geometry
            GetBreps(curves, voids);
        }

        private void GetInertias(List<Point3d> centroids, List<Vector3d> inertias, List<double> areas)
        {
            //Initialize
            Istrong = 0;
            Iweak = 0;

            //Distance indices
            int iDistStrong = Analysis.StrongDir(this.Plane);
            int iDistWeak = Analysis.WeakDir(this.Plane);

            for (int i = 0; i < centroids.Count; i++)
            {
                var cent2cent = Centroid - centroids[i];
                var inertia = inertias[i];

                Istrong += inertia[iDistWeak] + areas[i] * Math.Pow(cent2cent[iDistStrong], 2);
                Iweak += inertia[iDistStrong] + areas[i] * Math.Pow(cent2cent[iDistWeak], 2);
            }


            
        }

        private void GetBreps(List<Curve> solids, List<Curve> voids)
        {
            Geometry = new List<Curve>();
            foreach (Curve solid in solids)
            {
                Curve[] diffs = Curve.CreateBooleanDifference(solid, voids, Analysis.tol);

                if (diffs.Length > 0)
                {
                    foreach(Curve diff in diffs)
                    {
                        Geometry.Add(diff);
                    }
                }
                else
                {
                    Geometry.Add(solid);
                }
            }
        }

    }
}
