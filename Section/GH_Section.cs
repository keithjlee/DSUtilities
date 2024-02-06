using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
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
              "Create a cross section",
              "DSutilities", "Section")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Geometry", "Geo", "Section geometry as planar brep", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Plane", "Plane of geometry", GH_ParamAccess.item, Plane.WorldXY);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "Sec", "Section object", GH_ParamAccess.item);
            pManager.AddBrepParameter("BoundingBox", "BB", "Bounding box of object", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Analysis plane", "Analaysis plane", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Initialize
            Brep geo = null;
            Plane plane = Plane.WorldXY;

            if (!DA.GetData(0, ref geo)) return;
            DA.GetData(1, ref plane);

            Section section = new Section(geo, plane);

            DA.SetData(0, section);
            DA.SetData(1, section.LocalBox);
            DA.SetData(2, section.Plane);
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
            get { return new Guid("703482B5-2E8D-413D-B9D5-C8EF0A3662C6"); }
        }
    }
}