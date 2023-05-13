using System;
using System.Collections.Generic;
using System.ComponentModel;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace DSUtilities.Section
{
    public class GH_Section : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_Section class.
        /// </summary>
        public GH_Section()
          : base("Section", "Section",
              "A structural cross section",
              "DSutilities", "Section")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Solids", "Solids", "Closed curves representing solid regions", GH_ParamAccess.list);
            pManager.AddCurveParameter("Voids", "Voids", "Closed curves representing void regions", GH_ParamAccess.list);
            pManager.AddNumberParameter("YoungsModulus", "E", "Young's modulus of section, optional", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("AnalysisPlane", "Plane", "Plane for analysis", GH_ParamAccess.item, 0);

            pManager[1].Optional = true;

            Param_Integer param = pManager[3] as Param_Integer;
            param.AddNamedValue("XY", 0);
            param.AddNamedValue("YZ", 1);
            param.AddNamedValue("XZ", 2);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "Section", "A cross section", GH_ParamAccess.item);
            //pManager.AddNumberParameter("Area", "A", "Area of section", GH_ParamAccess.item);
            //pManager.AddPointParameter("Centroid", "Centroid", "Centroid of section", GH_ParamAccess.item);
            //pManager.AddNumberParameter("MomentInertia1", "I1", "Moment of inertia around axis 1", GH_ParamAccess.item);
            //pManager.AddNumberParameter("MomentInertia2", "I2", "Moment of inertia around axis 2", GH_ParamAccess.item);
            pManager.AddBrepParameter("Geometry", "Geo", "Final geometry of section", GH_ParamAccess.list);
            //pManager.AddLineParameter("BoundingBox", "BB", "Bounding box of section", GH_ParamAccess.list);

            //pManager.HideParameter(6);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //initialize
            List<Curve> solids = new List<Curve>();
            List<Curve> voids = new List<Curve>();
            double E = 1;
            int plane = 0;

            //assign
            if (!DA.GetDataList(0, solids)) return;
            DA.GetDataList(1, voids);
            DA.GetData(2, ref E);
            DA.GetData(3, ref plane);

            //go/no-go
            if (!Analysis.ValidCheck(solids)) return;

            if (voids.Count != 0)
            {
                if (!Analysis.ValidCheck(voids)) return;
                if (!Analysis.SolidVoidCheck(solids, voids, plane)) return;
            }

            

            //solve
            Analysis.AnalysisPlane refplane = (Analysis.AnalysisPlane)plane;
            Section section = new Section(solids, voids, E, refplane);

            DA.SetData(0, section);
            //DA.SetData(1, section.Area);
            //DA.SetData(2, section.Centroid);
            //DA.SetData(3, section.Istrong);
            //DA.SetData(4, section.Iweak);
            DA.SetDataList(1, section.Geometry);
            //DA.SetDataList(6, section.BoundingBox.GetEdges());
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.section;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6EAFC2FF-AA3C-4780-A524-F1AB8B105E21"); }
        }
    }
}