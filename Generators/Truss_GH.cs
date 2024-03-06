using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace DSUtilities.Generators
{
    public class Truss_GH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Truss_GH class.
        /// </summary>
        public Truss_GH()
          : base("Truss", "TrussCurve",
              "Turn a curve into a truss",
              "DSutilities", "Generators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "Crv", "Curve to turn into truss", GH_ParamAccess.item);
            pManager.AddIntegerParameter("NumberOfBays", "nBays", "Number of bays (spans) of truss", GH_ParamAccess.item, 3);
            pManager.AddNumberParameter("TrussDepth", "Depth", "Depth of truss", GH_ParamAccess.item, 1);
            pManager.AddVectorParameter("OffsetOverride", "Voffset", "Optional override vector for truss depth direction", GH_ParamAccess.item, new Vector3d(0, 0, 0));
            pManager.AddNumberParameter("RollAngle", "Angle", "Rotation of offset direction about curve tangent", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("Type", "Type", "Type of truss. 0: Warren, 1: Pratt/Howe", GH_ParamAccess.item, 0);

            Param_Integer param = pManager[5] as Param_Integer;
            param.AddNamedValue("Warren", 0);
            param.AddNamedValue("Pratt", 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Edges", "Edges", "Edges of truss", GH_ParamAccess.list);
            pManager.AddPointParameter("Nodes", "Nodes", "Nodes of truss", GH_ParamAccess.list);
            pManager.AddIntegerParameter("StartIndices", "iStart", "Indices of start nodes for edges", GH_ParamAccess.list);
            pManager.AddIntegerParameter("EndIndices", "iEnd", "Indices of end nodes for edges", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Chord1Nodes", "iChord1", "Indices of primary chord nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Chord2Nodes", "iChord2", "Indices of secondary chord nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Chord1Elements", "iChord1e", "Indices of primary chord elements", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Chord2Elements", "iChord2e", "Indices of secondary chord elements", GH_ParamAccess.list);
            pManager.AddIntegerParameter("WebElements", "iWeb", "Indices of web elements", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            int nbays = 3;
            double d = 1;
            Vector3d offset = new Vector3d(0, 0, 0);
            double angle = 0;
            int type = 0;

            if (!DA.GetData(0, ref curve)) return;
            DA.GetData(1, ref nbays);
            DA.GetData(2, ref d);
            DA.GetData(3, ref offset);
            DA.GetData(4, ref angle);
            DA.GetData(5, ref type);

            Truss truss = new Truss();

            if (type == 0)
            {
                truss = Truss.Warren(curve, nbays, d, offset, angle);
            }
            else if (type == 1)
            {
                if (nbays % 2 != 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Number of bays must be even for Pratt trusses, additional bay added");
                    nbays += 1;
                }
                truss = Truss.Pratt(curve, nbays, d, offset, angle);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Type not recongized. Can be 0 or 1. Defaulted to 0: Warren.");
                truss = Truss.Warren(curve, nbays, d, offset, angle);
            }


            DA.SetDataList(0, truss.Lines);
            DA.SetDataList(1, truss.Nodes);
            DA.SetDataList(2, truss.Istart);
            DA.SetDataList(3, truss.Iend);
            DA.SetDataList(4, truss.Ichord1nodes);
            DA.SetDataList(5, truss.Ichord2nodes);
            DA.SetDataList(6, truss.Ichord1elements);
            DA.SetDataList(7, truss.Ichord2elements);
            DA.SetDataList(8, truss.Iwebelements);
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
            get { return new Guid("AA22AB43-8E11-4BAD-89FD-2D3087CC13B5"); }
        }
    }
}