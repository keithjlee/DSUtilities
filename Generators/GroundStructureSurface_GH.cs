using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace DSUtilities.Generators
{
    public class GroundStructureSurface_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GroundStructureSurface_GH class.
        /// </summary>
        public GroundStructureSurface_GH()
          : base("GroundStructureSurface", "GroundStructSrf",
              "Generate a ground structure based on the UV frames of a surface",
              "DSutilities", "Generators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "Srf", "Surface for ground structure", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Nx", "Nx", "Number of nodes in X direction", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("Ny", "Ny", "Number of nodes in Y direction", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("Type", "Type", "Type of ground structure. 0: grid, 1: x-grid, 2: dense", GH_ParamAccess.item, 0);

            Param_Integer param = pManager[3] as Param_Integer;
            param.AddNamedValue("Grid", 0);
            param.AddNamedValue("X", 1);
            param.AddNamedValue("Dense", 2);

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
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface surface = null;
            int nx = 10;
            int ny = 10;
            int type = 0;

            if (!DA.GetData(0, ref surface)) return;
            DA.GetData(1, ref nx);
            DA.GetData(2, ref ny);
            DA.GetData(3, ref type);

            GroundStructure gs = new GroundStructure();
            if (type == 0)
            {
                gs = new GridGroundStructure(surface, nx, ny);
            }
            else if (type == 1)
            {
                gs = new XGroundStructure(surface, nx, ny);
            }
            else if (type == 2)
            {
                gs = new DenseGroundStructure(surface, nx, ny);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Type not recognized (can be 0/1/2), defaulted to 0: Grid");
                gs = new GridGroundStructure(surface, nx, ny);
            }

            //indices
            GroundStructureGeneration.GetPerimeterIndices(gs.Igrid, out List<int> ix1, out List<int> ix2, out List<int> iy1, out List<int> iy2);

            //indices
            List<int> i_interior = GroundStructureGeneration.GetInteriorIndices(gs.Igrid);

            DA.SetDataList(0, gs.Lines);
            DA.SetDataList(1, gs.Nodes);
            DA.SetDataList(2, gs.Istart);
            DA.SetDataList(3, gs.Iend);
            DA.SetDataList(4, ix1);
            DA.SetDataList(5, ix2);
            DA.SetDataList(6, iy1);
            DA.SetDataList(7, iy2);
            DA.SetDataList(8, i_interior);
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
            get { return new Guid("AEB72EC5-AA73-4CA7-A435-64C003AD0C9C"); }
        }
    }
}