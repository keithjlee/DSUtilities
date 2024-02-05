using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.CommonSections
{
    public class HSSRound : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the HSSRound class.
        /// </summary>
        public HSSRound()
          : base("HSSRound", "HSSRound",
              "Make a circular hollow section",
              "DSutilities", "SectionMaker")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Radius", "r", "Radius of section", GH_ParamAccess.item);
            pManager.AddNumberParameter("Thickness", "t", "Wall thickness", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Plane", "Plane origin of section", GH_ParamAccess.item, Plane.WorldXY);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Geometry", "Geo", "Geometry of section", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double r = 0;
            double t = 0;
            Plane plane = Plane.WorldXY;

            //assign
            if (!DA.GetData(0, ref r)) return;
            if (!DA.GetData(1, ref t)) return;
            DA.GetData(2, ref plane);

            Brep brep = SectionDrawer.MakeHSSRound(plane, r, t);
            DA.SetData(0, brep);
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
            get { return new Guid("8B4442DF-E90E-44CD-8951-3F316B7A5638"); }
        }
    }
}