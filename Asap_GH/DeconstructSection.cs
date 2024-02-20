using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DSUtilities.Asap;

namespace DSUtilities.Asap_GH
{
    public class DeconstructSection : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructSection class.
        /// </summary>
        public DeconstructSection()
          : base("DeconstructSection", "DeconSec",
              "Deconstruct an Asap section",
              "DSutilities", "Asap")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "Sec", "Section to deconstruct", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("YoungsModulus", "E", "Young's Modulus of section material", GH_ParamAccess.item);
            pManager.AddNumberParameter("ShearModulus", "G", "Shear Modulus of section material", GH_ParamAccess.item);
            pManager.AddNumberParameter("Area", "A", "Cross sectional area", GH_ParamAccess.item);
            pManager.AddNumberParameter("MomentOfInertia1", "Ix", "'strong' moment of inertia about local Z axis", GH_ParamAccess.item);
            pManager.AddNumberParameter("MomentOfInertia2", "Iy", "'weak' moment of inertia about local Y axis", GH_ParamAccess.item);
            pManager.AddNumberParameter("TorsionalConstant", "J", "Torsional constant", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Asap.Section section = new Asap.Section();
            if (!DA.GetData(0, ref section)) return;

            DA.SetData(0, section.E);
            DA.SetData(1, section.G);
            DA.SetData(2, section.A);
            DA.SetData(3, section.Ix);
            DA.SetData(4, section.Iy);
            DA.SetData(5, section.J);
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
            get { return new Guid("642A3EBD-3C19-4EDF-BDC6-E46B198B9C64"); }
        }
    }
}