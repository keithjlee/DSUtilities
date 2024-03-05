using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace DSUtilities.Generators
{
    public class SpaceFrameMesh_GH : GH_Component
    {
        public List<Point3d> Points = new List<Point3d>();
        public List<Line> Lines = new List<Line>();
        public List<int> Istart = new List<int>();
        public List<int> Iend = new List<int>();
        public List<int> Ibase = new List<int>();
        public List<int> Ioffset = new List<int>();
        public List<int> Ichord1 = new List<int>();
        public List<int> Ichord2 = new List<int>();
        public List<int> Iweb = new List<int>();

        /// <summary>
        /// Initializes a new instance of the SpaceFrameMesh_GH class.
        /// </summary>
        public SpaceFrameMesh_GH()
          : base("SpaceFrameMesh_GH", "SpaceFrameMesh",
              "Generate a spaceframe based on a mesh",
              "DSutilities", "Generators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh to convert to spaceframe", GH_ParamAccess.item);
            pManager.AddNumberParameter("Dz", "Dz", "Spaceframe offset", GH_ParamAccess.item, 1);
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
            pManager.AddIntegerParameter("iInterior", "iBase", "Indices of base nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iOffset", "iOffset", "Indices of offset  nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iChord1", "iChord1", "Indices of base plane elements", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iChord2", "iChord2", "Indices of offset plane elements", GH_ParamAccess.list);
            pManager.AddIntegerParameter("iWeb", "iWeb", "Indices of web elements", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            double dz = 0;

            if (!DA.GetData(0, ref mesh)) return;
            DA.GetData(1, ref dz);

            int index = 0;
        }

        private void PopulateChord1(Mesh mesh, int index)
        {
            MeshVertexList vertices = mesh.Vertices;



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
            get { return new Guid("D12713DA-C551-43F0-A318-3D4BC97DBEED"); }
        }
    }
}