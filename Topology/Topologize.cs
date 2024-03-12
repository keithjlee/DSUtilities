using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DSUtilities.Topology
{
    public class DSUtilities : GH_Component
    {

        public List<double> X;
        public List<double> Y;
        public List<double> Z;
        public List<int> Istarts;
        public List<int> Iends;
        public List<int> Inodes;
        int offset;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public DSUtilities()
          : base("Topologize", "Topo",
            "Extract the topology of a line network",
            "DSutilities", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "Curves", "Input curves", GH_ParamAccess.list);
            pManager.AddPointParameter("Nodes", "Nodes", "Nodes of interest", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scale", "Scale", "Scale factor for distance between nodes", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Tolerance", "Tol", "Search tolerance for shared nodes", GH_ParamAccess.item, 0.1);
            pManager.AddBooleanParameter("OneBased", "OneBased", "One based indexing?", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "Points", "Vertices of topology.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("StartIndices", "iStart", "Start indices of edges.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("EndIndices", "iEnd", "End indices of edges.", GH_ParamAccess.list);
            pManager.AddTextParameter("Topology", "Topology", "Topology of curves represented as a text JSON", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();
            List<Point3d> nodes = new List<Point3d>();
            double scale = 1.0;
            double tol = 1.0;
            bool indexoffset = false;

            if (!DA.GetDataList(0, curves)) return;
            
            DA.GetDataList(1, nodes);

            DA.GetData(2, ref scale);
            DA.GetData(3, ref tol);

            DA.GetData(4, ref indexoffset);

            if (indexoffset)
            {
                offset = 1;
            }
            else
            {
                offset = 0;
            }

            //create topology
            MakeTopology(curves, tol, scale, nodes);

            //make object
            TopologyObject topobj = new TopologyObject(X, Y, Z, Istarts, Iends, Inodes);

            var serialized = JsonConvert.SerializeObject(topobj);

            List<Point3d> points = topobj.ToPoints();

            DA.SetDataList(0, points);
            DA.SetDataList(1, Istarts);
            DA.SetDataList(2, Iends);
            DA.SetData(3, serialized);
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

        private void MakeTopology(List<Curve> edges, double tolerance, double factor, List<Point3d> nodes)
        {
            //initialize persistent variables
            X = new List<double>();
            Y = new List<double>();
            Z = new List<double>();
            Istarts = new List<int>();
            Iends = new List<int>();
            Inodes = new List<int>();

            //temporary variables
            Point3dList pointlist = new Point3dList();

            foreach (Curve edge in edges)
            {
                Point3d start = edge.PointAtStart * factor;
                Point3d end = edge.PointAtEnd * factor;
                int istart;
                int iend;

                if (!WithinTolerance(pointlist, start, tolerance))
                {
                    pointlist.Add(start);
                    istart = pointlist.Count - 1 + offset;

                    X.Add(start.X);
                    Y.Add(start.Y);
                    Z.Add(start.Z);
                }
                else
                {
                    istart = pointlist.ClosestIndex(start) + offset;
                }

                // analyze end point
                //if (!pointlist.Contains(end))
                if (!WithinTolerance(pointlist, end, tolerance))
                {
                    pointlist.Add(end);
                    iend = pointlist.Count - 1 + offset;

                    X.Add(end.X);
                    Y.Add(end.Y);
                    Z.Add(end.Z);
                }
                else
                {
                    iend = pointlist.ClosestIndex(end) + offset;
                }

                Istarts.Add(istart);
                Iends.Add(iend);


            }

            foreach (Point3d node in nodes)
            {
                var inode = pointlist.ClosestIndex(node) + offset;
                Inodes.Add(inode);
            }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.topologize;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("f4421c22-cbed-47a5-9344-08128731e045");
    }
}