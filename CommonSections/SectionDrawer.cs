using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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
            GetPlaneInfo(plane, out Point3d O, out Vector3d X, out Vector3d Y);

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
            GetPlaneInfo(plane, out Point3d O, out Vector3d X, out Vector3d Y);

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

        public static Curve MakeTee(Plane plane, double d, double b, double tf, double tw)
        {
            // extract LCS
            GetPlaneInfo(plane, out Point3d O, out Vector3d X, out Vector3d Y);

            // make points
            Point3d p1 = O - Y * (d - tf); //bottom middle  of web
            Point3d p2 = p1 - X * (tw / 2); //bottom left of web
            Point3d p3 = p2 + Y * (d - tf); //flange-web interface
            Point3d p4 = p3 - X * (b - tw) / 2; //bottom left of flange
            Point3d p5 = p4 + Y * tf; //top left of flange
            Point3d p6 = p5 + X * (b / 2); //top middle of flange


            PolylineCurve left = new PolylineCurve(new List<Point3d> { p1, p2, p3, p4, p5, p6 });
            var right = (PolylineCurve)left.Duplicate();

            //mirror
            Plane flip_plane = new Plane(O, X);
            Transform flip_transform = Transform.Mirror(flip_plane);
            right.Transform(flip_transform);

            //join
            Curve section = Curve.JoinCurves(new List<Curve> { left, right })[0];

            return section;

        }

        public static Curve MakeAngle(Plane plane, double d, double b, double t)
        {
            // extract LCS
            GetPlaneInfo(plane, out Point3d O, out Vector3d X, out Vector3d Y);

            // make points
            Point3d p1 = O + Y * d; //top left
            Point3d p2 = p1 + X * t;
            Point3d p3 = p2 - Y * (d - t);
            Point3d p4 = p3 + X * (b - t);
            Point3d p5 = p4 - Y * t;

            // make curve
            PolylineCurve section = new Polyline(new List<Point3d> { O, p1, p2, p3, p4, p5, O }).ToPolylineCurve();

            return section;

        }

        public static Brep MakeHSSRound(Plane plane, double r, double t)
        {
            Curve outer = new Circle(plane, r).ToNurbsCurve();
            Brep outer_brep = Brep.CreatePlanarBreps(outer, 1e-6)[0];

            Curve inner = new Circle(plane, r - t).ToNurbsCurve();
            Brep inner_brep = Brep.CreatePlanarBreps(inner, 1e-6)[0];

            Brep HSS = Brep.CreateBooleanDifference(outer_brep, inner_brep, 1e-6)[0];

            return HSS;
        }

        public static Brep MakeHSSRect(Plane plane, double d, double b, double t)
        {
            // extract LCS
            GetPlaneInfo(plane, out Point3d O, out Vector3d X, out Vector3d Y);

            Point3d corner_outer_1 = O - X * b / 2 - Y * d / 2;
            Point3d corner_outer_2 = O + X * b / 2 + Y * d / 2;
            Curve outer = new Rectangle3d(plane, corner_outer_1, corner_outer_2).ToNurbsCurve();
            Brep outer_brep = Brep.CreatePlanarBreps(outer, 1e-6)[0];

            Point3d corner_inner_1 = O - X * (b / 2 - t) - Y * (d / 2 - t);
            Point3d corner_inner_2 = O + X * (b / 2 - t) + Y * (d / 2 - t);
            Curve inner = new Rectangle3d(plane, corner_inner_1, corner_inner_2).ToNurbsCurve();
            Brep inner_brep = Brep.CreatePlanarBreps(inner, 1e-6)[0];

            Brep HSS = Brep.CreateBooleanDifference(outer_brep, inner_brep, 1e-6)[0];

            return HSS;
        }

        public static void GetPlaneInfo(Plane plane, out Point3d O, out Vector3d X, out Vector3d Y)
        {
            O = plane.Origin;
            X = plane.XAxis;
            Y = plane.YAxis;
        }
    }
}
