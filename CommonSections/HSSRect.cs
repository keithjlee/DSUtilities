using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.CommonSections
{
    public class HSSRect : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HSSRect class.
        /// </summary>
        public HSSRect()
          : base("HSSRect", "HSSRect",
              "Make a rectangular hollow section",
              "DSutilities", "SectionMaker")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Depth", "d", "Depth of rectangle", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "b", "Width of rectangle", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "t", "Wall thickness", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Plane", "Plane origin of section", GH_ParamAccess.item, Plane.WorldXY);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Geometry", "Geo", "Geometry of section", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Plane", "Plane of analysis for section", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double d = 0;
            double b = 0;
            double t = 0;
            Plane plane = Plane.WorldXY;

            //assign
            if (!DA.GetData(0, ref d)) return;
            if (!DA.GetData(1, ref b)) return;
            if (!DA.GetData(2, ref t)) return;
            DA.GetData(3, ref plane);

            Brep brep = SectionDrawer.MakeHSSRect(plane, d, b, t);

            // set output plane
            Plane outplane = new Plane(AreaMassProperties.Compute(brep).Centroid, plane.XAxis, plane.YAxis);

            DA.SetData(0, brep);
            DA.SetData(1, outplane);
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
            get { return new Guid("3CC7CC84-184F-4E62-B082-D5FF39EE6109"); }
        }
    }
}