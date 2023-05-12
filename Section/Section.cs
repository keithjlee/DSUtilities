using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using MathNet.Numerics;

namespace DSUtilities.Section
{
    internal class Section
    {
        public double Area;
        public Point3d Centroid;
        public Vector3d Inertias;
        public List<double> SectionModulii;
        public BoundingBox BoundingBox;
        public double E = 1;
        public List<Curve> Solids;
        public List<Curve> Voids;

        public Section(List<Curve> curves, List<Curve> voids, double E)
        {
            //go/no-go
            if (!Analysis.ValidCheck(curves)) return;
            if (!Analysis.ValidCheck(voids)) return;

            //initialize
            Area = 0;
            Centroid = new Point3d();
            Solids = curves;
            Voids = voids;
            this.E = E;

            
            //individual properties
            var areas = new List<double>();
            var centroids = new List<Point3d>();
            var inertias = new List<Vector3d>();

            List<Curve> allcurves = curves.Concat(voids).ToList();

            foreach (Curve curve in curves)
            {
                var properties = AreaMassProperties.Compute(curve);

                areas.Add(properties.Area);
                Area += properties.Area;

                centroids.Add(properties.Centroid);
                Centroid += properties.Area * properties.Centroid;

                inertias.Add(properties.CentroidCoordinatesMomentsOfInertia);
            }

            foreach (Curve curve in voids)
            {
                var properties = AreaMassProperties.Compute(curve);

                areas.Add(-properties.Area);
                Area -= properties.Area;

                centroids.Add(properties.Centroid);
                Centroid += -properties.Area * properties.Centroid;
            }

            //get total centroid
            Centroid /= Area;

            // compound moment of inertia
            double Ix = 0;
            double Iy = 0;
            double Iz = 0;

            for (int i = 0; i < allcurves.Count; i++)
            {
                var cent2cent = Centroid - centroids[i];
                Ix += inertias[i][0] + areas[i] * (Math.Pow(cent2cent.Y, 2) + Math.Pow(cent2cent.Z, 2));
                Iy += inertias[i][1] + areas[i] * (Math.Pow(cent2cent.X, 2) + Math.Pow(cent2cent.Z, 2));
                Iz += inertias[i][2] + areas[i] * (Math.Pow(cent2cent.X, 2) + Math.Pow(cent2cent.Y, 2));
            }

            Inertias = new Vector3d(Ix, Iy, Iz);

            //get bounding box
            this.BoundingBox = Analysis.GetBB(curves);
        }


    }
}
