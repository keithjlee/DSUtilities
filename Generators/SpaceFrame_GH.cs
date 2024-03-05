using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Generators
{
    public class SpaceFrame_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SpaceFrame_GH class.
        /// </summary>
        public SpaceFrame_GH()
          : base("SpaceFramePlane", "SpaceFramePlane",
              "Generate a spaceframe based on a plane",
              "DSutilities", "Generators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "Pl", "Plane of analysis", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddIntegerParameter("Nx", "Nx", "Number of nodes in X direction", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Dx", "Dx", "Distance in X direction", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Ny", "Ny", "Number of nodes in Y direction", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Dy", "Dy", "Distance in Y direction", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Dz", "Dz", "Spaceframe depth", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Edges", "Edges", "Edges of structure", GH_ParamAccess.list);
            pManager.AddPointParameter("Nodes", "Nodes", "Nodes of structure", GH_ParamAccess.list);
            pManager.AddIntegerParameter("StartIndices", "iStart", "Indices of start nodes for edges", GH_ParamAccess.list);
            pManager.AddIntegerParameter("EndIndices", "iEnd", "Indices of end nodes for edges", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iX1", "iX1", "Indices of first row of perimeter nodes in X direction", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iX2", "iX2", "Indices of second row of perimeter nodes in X direction", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iY1", "iY1", "Indices of first column of perimeter nodes in Y direction", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iY2", "iY2", "Indices of second column of perimeter nodes in Y direction", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iInterior", "iInt", "Indices of interior nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iOffset", "iOffset", "Indices of offset plane nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iChord1", "iBase", "Indices of base plane elements", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iChord2", "iOffset", "Indices of offset plane elements", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iWeb", "iWeb", "Indices of web elements", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //initialize
            Plane plane = Plane.WorldXY;
            int nx = 10;
            double dx = 1;
            int ny = 10;
            double dy = 1;
            double dz = 0;

            //populate
            DA.GetData(0, ref plane);
            DA.GetData(1, ref nx);
            DA.GetData(2, ref dx);
            DA.GetData(3, ref ny);
            DA.GetData(4, ref dy);
            DA.GetData(5, ref dz);

            SpaceFrame sf = new SpaceFrame(plane, nx, dx, ny, dy, dz);

            //perimeter nodes of base plane
            GroundStructureGeneration.GetPerimeterIndices(sf.Igrid1, out List<int> ix1, out List<int> ix2, out List<int> iy1, out List<int> iy2);

            //interior nodes of base plane
            List<int> i_interior = GroundStructureGeneration.GetInteriorIndices(sf.Igrid1);

            //nodes of offset plane
            List<int> i_offset = sf.Igrid2.Cast<int>().ToList();

            DA.SetDataList(0, sf.Lines);
            DA.SetDataList(1, sf.Nodes);
            DA.SetDataList(2, sf.Istart);
            DA.SetDataList(3, sf.Iend);
            DA.SetDataList(4, ix1);
            DA.SetDataList(5, ix2);
            DA.SetDataList(6, iy1);
            DA.SetDataList(7, iy2);
            DA.SetDataList(8, i_interior);
            DA.SetDataList(9, i_offset);
            DA.SetDataList(10, sf.Ichord1);
            DA.SetDataList(11, sf.Ichord2);
            DA.SetDataList(12, sf.Iweb);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("CCAB6C67-A7F8-47ED-A5A0-7318E3881E2B"); }
        }
    }
}