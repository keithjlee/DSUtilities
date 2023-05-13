using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Collections;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace DSUtilities.Topology
{
    public class AutoShatter : GH_Component
    {
        public Point3dList IntersectionPoints;
        public List<Curve> ShatteredCurves;
        public List<Curve> AllCurves;

        /// <summary>
        /// Initializes a new instance of the AutoShatter class.
        /// </summary>
        public AutoShatter()
          : base("AutoShatter", "AutoShatter",
              "Shatter lines into segments where intersections occur",
              "DSutilities", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "Curves", "Input curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "Tol", "Search tolerance", GH_ParamAccess.item, 0.1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Shattered Curves", "ShatteredCurves", "Shattered curve results", GH_ParamAccess.list);
            pManager.AddPointParameter("Shatter Points", "Nodes", "Points where shattering occured", GH_ParamAccess.list);
            pManager.AddCurveParameter("All Curves", "AllCurves", "All curves, including non-shattered",
                   GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Initialize
            List<Curve> curves = new List<Curve>();
            double tol = 0.1;


            // Assign
            if (!DA.GetDataList(0, curves)) return;
            DA.GetData(1, ref tol);

            Shatter(curves, tol);

            DA.SetDataList(0, ShatteredCurves);
            DA.SetDataList(1, IntersectionPoints);
            DA.SetDataList(2, AllCurves);

        }

        private void Shatter(List<Curve> curves, double tol)
        {
            // Computation
            IntersectionPoints = new Point3dList();
            ShatteredCurves = new List<Curve>();
            AllCurves = new List<Curve>();

            List<List<double>> paramset = new List<List<double>>();
            for (int i = 0; i < curves.Count; i++)
            {
                paramset.Add(new List<double>());
            }

            for (int i = 0; i < curves.Count - 1; i++)
            {
                // curve of interest
                Curve curveA = curves[i];

                for (int j = i + 1; j < curves.Count; j++)
                {
                    Curve curveB = curves[j];
                    var events = Intersection.CurveCurve(curveA, curveB, tol, tol);

                    if (events != null)
                    {
                        for (int k = 0; k < events.Count; k++)
                        {
                            var intevent = events[k];
                            var pointonA = intevent.PointA;
                            var parA = intevent.ParameterA;
                            var parB = intevent.ParameterB;

                            // if this is a new intersection point, add to collection
                            if (!WithinTolerance(IntersectionPoints, pointonA, tol))
                            {
                                IntersectionPoints.Add(pointonA);
                            }

                            // shatter curve A if not at end points
                            if (parA > tol && curveA.GetLength() - parA > tol)
                            {
                                paramset[i].Add(parA);
                            }

                            if (parB > tol && curveB.GetLength() - parB > tol)
                            {
                                paramset[j].Add(parB);
                            }
                        }
                    }
                }


            }

            for (int i = 0; i < curves.Count; i++)
            {
                var ppoints = paramset[i];

                if (ppoints.Count > 0)
                {
                    var newcurves = curves[i].Split(ppoints);

                    foreach (Curve curve in newcurves)
                    {
                        ShatteredCurves.Add(curve);
                        AllCurves.Add(curve);
                    }
                }
                else
                {
                    AllCurves.Add(curves[i]);
                }
            }

        }

        private bool WithinTolerance(Point3dList points, Point3d point, double tolerance)
        {
            try
            {
                double dist = point.DistanceTo(Point3dList.ClosestPointInList(points, point));

                if (dist < tolerance) return true;
                else return false;
            }
            catch
            {
                return false;
            }

        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.autoshatter;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("97F9B192-12E5-4958-B33F-2AB6034875E1"); }
        }
    }
}