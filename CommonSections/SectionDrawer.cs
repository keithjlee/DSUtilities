using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace DSUtilities.CommonSections
{
    internal static class SectionDrawer
    {
        public static Curve MakeIBeam(Plane plane, double d, double b, double tf, double tw)
        {
            // extract LCS
            Point3d O = plane.Origin;
            Vector3d X = plane.XAxis;
            Vector3d Y = plane.YAxis;

            // make points
            Point3d p1 = O - X * tw / 2;
            Point3d p2 = p1 + Y * (d / 2 - tf);
            Point3d p3 = p2 - X * (b - tw) / 2;
            Point3d p4 = p3 + Y * tf;
            Point3d p5 = O + Y * d / 2;

            List<Point3d> quadrant = new List<Point3d> { p1, p2, p3, p4, p5 };

            // make top left quadrant curve
            PolylineCurve top_left = new PolylineCurve(quadrant);

            // vertical mirror transform
            PolylineCurve bottom_left = (PolylineCurve) top_left.Duplicate();

            // flip plane
            Plane flip_plane_1 = new Plane(O, Y);
            Transform flip_transform_1 = Transform.Mirror(flip_plane_1);

            // mirror
            bottom_left.Transform(flip_transform_1);

            // join
            Curve left = Curve.JoinCurves(new List<PolylineCurve> { top_left, bottom_left })[0];

            //copy
            Curve right = (Curve)left.Duplicate();

            // flip
            Plane flip_plane_2 = new Plane(O, -X);
            Transform flip_transform_2 = Transform.Mirror(flip_plane_2);

            //mirror
            right.Transform(flip_transform_2);

            //join
            Curve section = Curve.JoinCurves(new List<Curve> { left, right })[0];

            return section;
           
        }

        public static Curve MakeChannel(Plane plane, double d, double b, double t)
        {
            // extract LCS
            Point3d O = plane.Origin;
            Vector3d X = plane.XAxis;
            Vector3d Y = plane.YAxis;

            // make points
            Point3d p1 = O - X * t / 2;
            Point3d p2 = p1 + Y * d / 2;
            Point3d p3 = p2 + X * b;
            Point3d p4 = p3 - Y * t;
            Point3d p5 = p4 - X * (b - t);
            Point3d p6 = p5 - Y * (d / 2 - t);

            PolylineCurve top = new PolylineCurve(new List<Point3d> { p1, p2, p3, p4, p5, p6 });

            //mirror
            PolylineCurve bottom = (PolylineCurve)top.Duplicate();
            Plane flip_plane = new Plane(O, Y);
            Transform flip_transform = Transform.Mirror(flip_plane);
            bottom.Transform(flip_transform);

            //join
            Curve section = Curve.JoinCurves(new List<Curve> { top, bottom })[0];

            return section;
        }
    }
}
