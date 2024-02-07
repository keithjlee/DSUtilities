using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.CommonSections
{
    public class IBeam : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IBeam class.
        /// </summary>
        public IBeam()
          : base("IBeam", "I",
              "Make a flanged I beam",
              "DSutilities", "SectionMaker")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Depth", "d", "Depth of section", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "b", "Width of flange", GH_ParamAccess.item);
            pManager.AddNumberParameter("tFlange", "tf", "Flange thickness", GH_ParamAccess.item);
            pManager.AddNumberParameter("tWeb", "tw", "Web thickness", GH_ParamAccess.item);
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
            //Initialize
            double d = 0;
            double b = 0;
            double tf = 0;
            double tw = 0;
            Plane plane = Plane.WorldXY;

            // assign
            if (!DA.GetData(0, ref d)) return;
            if (!DA.GetData(1, ref b)) return;
            if (!DA.GetData(2, ref tf)) return;
            if (!DA.GetData(3, ref tw)) return;
            DA.GetData(4, ref plane);

            // calculate
            Curve curve = SectionDrawer.MakeIBeam(plane, d, b, tf, tw);
            Brep brep = Brep.CreatePlanarBreps(curve, 1e-6)[0];

            // set output plane
            Plane outplane = new Plane(AreaMassProperties.Compute(brep).Centroid, plane.XAxis, plane.YAxis);

            DA.SetData(0, brep);
            DA.SetData(1, outplane);

            //

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
            get { return new Guid("30A649EA-A8C6-4E46-B181-7A6CBD6FE1C3"); }
        }
    }
}