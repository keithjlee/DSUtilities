using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using MathNet.Numerics.RootFinding;
using Rhino.Geometry;

namespace DSUtilities.Section
{
    public class AfromD : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AfromC class.
        /// </summary>
        public AfromD()
          : base("AreaFromDepth", "AfromD",
              "Get the cross section area at a depth D",
              "DSutilities", "Section")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "Section", "Section to analyze", GH_ParamAccess.item);
            pManager.AddNumberParameter("Depth", "d", "Depth of analysis", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Direction", "Direction", "Direction of depth (-1: top, 1:bottom)", GH_ParamAccess.item, -1);

            Param_Integer param = pManager[2] as Param_Integer;
            param.AddNamedValue("Downwards", -1);
            param.AddNamedValue("Upwards", 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Area", "A", "Area from depth", GH_ParamAccess.item);
            pManager.AddCurveParameter("Region", "Region", "Region of analysis", GH_ParamAccess.item) ;
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Section section = new Section();
            double depth = 0;
            int dir = -1;

            if (!DA.GetData(0, ref section)) return;
            if (!DA.GetData(1, ref depth)) return;
            DA.GetData(2, ref dir);


            //analysis
            Vector3d vec = Analysis.UnitVector(section.Plane);
            Plane referenceplane = Analysis.APtoWorld((int)section.Plane);
            Point3d startpoint;
            Point3d endpoint;
            //extract relevant corner points
            if (dir == -1)
            {
                startpoint = section.Corners[0];
                endpoint = section.Corners[1] - depth * vec;
            }
            else
            {
                startpoint = section.Corners[2];
                endpoint = section.Corners[3] + depth * vec;
            }

            Curve region = new Rectangle3d(referenceplane, startpoint, endpoint).ToNurbsCurve();

            DA.SetData(0, Analysis.GetArea(section, region));
            DA.SetData(1, region);


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AfromD;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7EDC33A3-512F-44FA-8B42-8B58F8EE766E"); }
        }
    }
}