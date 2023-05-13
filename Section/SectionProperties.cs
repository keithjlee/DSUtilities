using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.Section
{
    public class SectionProperties
    {
        public double Area;
        public Point3d Centroid;
        public double Istrong;
        public double Iweak;
        public double Sstrong1;
        public double Sstrong2;
        public double Sweak1;
        public double Sweak2;
        //public double Zstrong;
        //public double Zweak;
        public double H;
        public double W;

        public SectionProperties(Section section)
        {
            Area = section.Area;
            Centroid = section.Centroid;
            Istrong = section.Istrong;
            Iweak = section.Iweak;
            H = section.Corners[0].DistanceTo(section.Corners[2]);
            W = section.Corners[0].DistanceTo(section.Corners[1]);

            GetS(section);
            //GetZ(section, tol, maxiter);

        }

        private void GetS(Section section)
        {
            //section modulus
            int iDistStrong = Analysis.StrongDir(section.Plane);
            int iDistWeak = Analysis.WeakDir(section.Plane);

            //strong axis
            var x1 = Math.Abs((section.Corners[0] - section.Centroid)[iDistStrong]);
            var x2 = Math.Abs((section.Corners[2] - section.Centroid)[iDistStrong]);

            Sstrong1 = Istrong / x1;
            Sstrong2 = Istrong / x2;

            //weak axis
            var y1 = Math.Abs((section.Corners[1] - section.Centroid)[iDistWeak]);
            var y2 = Math.Abs((section.Corners[0] - section.Centroid)[iDistWeak]);

            Sweak1 = Iweak / y1;
            Sweak2 = Iweak / y2;
        }

        private void GetZ(Section section, double tol, int maxiter)
        {
            //section modulus
            int iDistStrong = Analysis.StrongDir(section.Plane);
            int iDistWeak = Analysis.WeakDir(section.Plane);

            double Atarget = Area / 2;
            Plane refplane = Analysis.APtoWorld((int)section.Plane);
            Vector3d vec = Analysis.UnitVector(section.Plane);

            Point3d startpoint = section.Corners[0];
            Point3d endpoint = section.Corners[1];

            //strong axis analysis
            double dupper = H;
            double dlower = 0;
            double area = 0;
            double err = Math.Abs(Atarget - area) / Atarget;
            double dstrong = (dupper + dlower) / 2;
            int iter = 1;

            Rectangle3d intersector = new Rectangle3d(refplane, startpoint, endpoint - dstrong * vec);

            while (iter <= maxiter || err > tol)

            {
                area = Analysis.GetArea(section, intersector.ToNurbsCurve());

                double diff = Atarget - area;

                err = Math.Abs(diff) / Atarget;

                if (diff > 0)
                {
                    dlower = dstrong;
                }
                else
                {
                    dupper = dstrong;
                }

                dstrong = (dupper + dlower) / 2;

                intersector = new Rectangle3d(refplane, startpoint, endpoint - dstrong * vec);

                iter++;
            }

            Point3d cstrong1 = Analysis.GetOverlapCentroid(section, intersector.ToNurbsCurve());

            Rectangle3d offsetrect = new Rectangle3d(refplane, startpoint - vec * dstrong, section.Corners[3]);

            Point3d cstrong2 = Analysis.GetOverlapCentroid(section, offsetrect.ToNurbsCurve());

            Zstrong = area * (Math.Abs(cstrong1[iDistStrong] - dstrong) + Math.Abs(cstrong2[iDistStrong] - dstrong));

            //weak axis
            dupper = W;
            dlower = 0;
            area = 0;
            err = Math.Abs(Atarget - area) / Atarget;
            double dweak = (dupper + dlower) / 2;
            iter = 1;
            vec = Analysis.UnitVectorWidth(section.Plane);
            endpoint = section.Corners[2];
            intersector = new Rectangle3d(refplane, startpoint, endpoint + dweak * vec);

            while (iter <= maxiter || err > tol)

            {
                area = Analysis.GetArea(section, intersector.ToNurbsCurve());

                double diff = Atarget - area;

                err = Math.Abs(diff) / Atarget;

                if (diff > 0)
                {
                    dlower = dweak;
                }
                else
                {
                    dupper = dweak;
                }

                dweak = (dupper + dlower) / 2;

                intersector = new Rectangle3d(refplane, startpoint, endpoint + dweak * vec);

                iter++;
            }

            Point3d cweak1 = Analysis.GetOverlapCentroid(section, intersector.ToNurbsCurve());

            offsetrect = new Rectangle3d(refplane, startpoint + vec * dweak, section.Corners[3]);

            Point3d cweak2 = Analysis.GetOverlapCentroid(section, offsetrect.ToNurbsCurve());

            Zweak = area * (Math.Abs(cweak1[iDistWeak] - dweak) + Math.Abs(cweak2[iDistWeak] - dweak));

        }
    }
}
