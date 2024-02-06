using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Section
{
    public class GH_SectionProperties : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GH_SectionProperties class.
        /// </summary>
        public GH_SectionProperties()
          : base("SectionProperties", "SecProps",
              "Properties of a section",
              "DSutilities", "Section")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "Sec", "Section to analyze", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Geometry", "Geo", "Geometry of section", GH_ParamAccess.item);
            pManager.AddBrepParameter("BoundingBox", "BB", "Bounding box of section", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Plane", "Analysis plane", GH_ParamAccess.item);
            pManager.AddPointParameter("Centroid", "C", "Centroid of section", GH_ParamAccess.item);
            pManager.AddNumberParameter("Depth", "D", "Depth of section", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "Width of section", GH_ParamAccess.item);
            pManager.AddNumberParameter("Area", "A", "Area of section", GH_ParamAccess.item);
            pManager.AddNumberParameter("Ix", "Ix", "Moment of inertia about local X axis", GH_ParamAccess.item);
            pManager.AddNumberParameter("Iy", "Iy", "Moment of inertia about local Y axis", GH_ParamAccess.item);
            pManager.AddNumberParameter("Sx1", "Sx1", "Section modulus about local X axis (top)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Sx2", "Sx2", "Section modulus about local X axis (bottom)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Sy1", "Sy1", "Section modulus about local Y axis (left)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Sy2", "Sy2", "Section modulus about local Y axis (right)", GH_ParamAccess.item);

            pManager.HideParameter(1);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Section section = new Section();

            if (!DA.GetData(0, ref section)) return;

            // populate
            DA.SetData(0, section.Geo);
            DA.SetData(1, section.LocalBox);
            DA.SetData(2, section.Plane);
            DA.SetData(3, section.Centroid);
            DA.SetData(4, section.Depth);
            DA.SetData(5, section.Width);
            DA.SetData(6, section.Area);
            DA.SetData(7, section.Ix);
            DA.SetData(8, section.Iy);
            DA.SetData(9, section.Sx1);
            DA.SetData(10, section.Sx2);
            DA.SetData(11, section.Sy1);
            DA.SetData(12, section.Sy2);
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
            get { return new Guid("D813AE48-C845-4740-BA23-727926E258CF"); }
        }
    }
}