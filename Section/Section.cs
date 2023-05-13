using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using MathNet.Numerics;
using Rhino.Collections;

namespace DSUtilities.Section
{
    public abstract class AbstractSection
    {
        public double Area;
        public Point3d Centroid;
        public double Istrong;
        public double Iweak;
        public Analysis.AnalysisPlane Plane;
        public BoundingBox BoundingBox;
        public List<Point3d> Corners;
        public List<Curve> Solids;
        public List<Curve> Voids;
        public List<Brep> Geometry;
    }
    public class Section: AbstractSection
    {
        public double E = 1;

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

            //get corners in relevant plane: top left, top right, bottom left,  bottom right
            this.Corners = Analysis.CornerPoints(this);

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

        //private void GetBreps(List<Curve> solids, List<Curve> voids)
        //{
        //    Geometry = new List<Curve>();
        //    foreach (Curve solid in solids)
        //    {
        //        Curve[] diffs = Curve.CreateBooleanDifference(solid, voids, Analysis.tol);

        //        if (diffs.Length > 0)
        //        {
        //            foreach(Curve diff in diffs)
        //            {
        //                Geometry.Add(diff);
        //            }
        //        }
        //        else
        //        {
        //            Geometry.Add(solid);
        //        }
        //    }
        //}

        private void GetBreps(List<Curve> solids, List<Curve> voids)
        {
            var solidbreps = Brep.CreatePlanarBreps(new CurveList(solids), Analysis.tol).ToList();

            if (voids.Count == 0)
            {
                //Geometry = new List<Brep>();
                //foreach (Curve solid in solids)
                //{
                //    Geometry.Add(Brep.TryConvertBrep(solid));
                //}

                Geometry = solidbreps;
            }
            else
            {
                var voidbreps = Brep.CreatePlanarBreps(new CurveList(voids), Analysis.tol).ToList();

                Geometry = Brep.CreateBooleanDifference(solidbreps, voidbreps, Analysis.tol, true).ToList();
            }
        }
    }

    /// <summary>
    /// Effective section of a series of multimaterial sections
    /// </summary>
    public class MultiSection : AbstractSection
    {
        public List<double> E;
        public List<Section> Sections;

        public MultiSection() { }

        public MultiSection(Section basesection, List<Section> sections)
        {
            // simple translations
            this.Plane = basesection.Plane;

            Sections = new List<Section> { basesection };
            Sections.AddRange(sections);

            E = new List<double> { basesection.E };
            E.AddRange(sections.Select(x => x.E).ToList());

            Area = basesection.Area;
            Centroid = basesection.Centroid * basesection.Area;

            BoundingBox = basesection.BoundingBox;
            Solids = new List<Curve>(basesection.Solids);
            Voids = new List<Curve>(basesection.Voids);
            Geometry = new List<Brep>(basesection.Geometry);

            List<Point3d> centroids = new List<Point3d> { basesection.Centroid };
            List<double> areas = new List<double> { basesection.Area };
            List<double> scalefactors = new List<double> { 1 };
            List<double> I1 = new List<double>{  basesection.Istrong};
            List<double> I2 = new List<double> { basesection.Iweak};

            

            //populate
            foreach (Section section in sections)
            {
                Area += section.Area;

                double SF = section.E / basesection.E;

                Centroid += section.Centroid * section.Area * SF;
                scalefactors.Add(SF);

                BoundingBox.Union(section.BoundingBox);
                Solids.AddRange(section.Solids);
                Voids.AddRange(section.Voids);
                Geometry.AddRange(section.Geometry);

                centroids.Add(section.Centroid);
                areas.Add(section.Area);
                I1.Add(section.Istrong);
                I2.Add(section.Iweak);
            }

            //normalize centroid
            Centroid /= Area;

            //moments of inertia
            GetInertias(centroids, areas, I1, I2, scalefactors);

            //corners
            Corners = Analysis.CornerPoints(this.BoundingBox, this.Plane);

        }

        private void GetInertias(List<Point3d> centroids, List<double> areas, List<double> inertias1, List<double> inertias2, List<double> scalefactors)
        {
            

            //Distance indices
            int iDistStrong = Analysis.StrongDir(this.Plane);
            int iDistWeak = Analysis.WeakDir(this.Plane);

            for (int i = 0; i < centroids.Count; i++)
            {
                var cent2cent = Centroid - centroids[i];
                var inertia1 = inertias1[i];
                var inertia2 = inertias2[i];
                var sf = scalefactors[i];

                Istrong += inertia1 + areas[i] * Math.Pow(cent2cent[iDistStrong], 2) * sf;
                Iweak += inertia2 + areas[i] * Math.Pow(cent2cent[iDistWeak], 2) * sf;
            }
        }
    }
}
