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
          : base("SectionProperties", "SectionProps",
              "Get all geometric section properties",
              "DSutilities", "Section")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "Section", "Section to analyze", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tolerance", "Tol", "Tolerance for Z calculations", GH_ParamAccess.item, Analysis.tol);
            pManager.AddIntegerParameter("MaxIter", "MaxIter", "Maximum iterations for Z calculations", GH_ParamAccess.item, 50);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Centroid", "Centroid", "Centroid of section", GH_ParamAccess.item);
            pManager.AddNumberParameter("Area", "A", "Area of section", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Height of section", GH_ParamAccess.item) ;
            pManager.AddNumberParameter("Width", "W", "Width of section", GH_ParamAccess.item);
            pManager.AddNumberParameter("Istrong", "I1", "Moment of inertia in canonical strong direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("Iweak", "I2", "Moment of inertia in canonical weak direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("SstrongTop", "Sstrong1", "Section modulus to top fiber in strong direction", GH_ParamAccess.item) ;
            pManager.AddNumberParameter("SstrongBottom", "Sstrong2", "Section modulus to bottom fiber in strong direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("SweakRight", "Sweak1", "Section modulus to leftmost fiber in weak direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("SweakLeft", "Sweak2", "Section modulus to rightmost fiber in weak direction", GH_ParamAccess.item);
            //pManager.AddNumberParameter("Zstrong", "Z1", "Plastic modulus in canonical strong direction", GH_ParamAccess.item);
            //pManager.AddNumberParameter("Zweak", "Z2", "Plastic modulus in canonical weak direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Section section = new Section();
            double tol = Analysis.tol;
            int maxiter = 100;

            if (!DA.GetData(0, ref section)) return;
            DA.GetData(1, ref tol);
            DA.GetData(2, ref maxiter);

            SectionProperties secprops = new SectionProperties(section);

            DA.SetData(0, secprops.Centroid);
            DA.SetData(1, secprops.Area);
            DA.SetData(2, secprops.H);
            DA.SetData(3, secprops.W);
            DA.SetData(4, secprops.Istrong);
            DA.SetData(5, secprops.Iweak);
            DA.SetData(6, secprops.Sstrong1);
            DA.SetData(7, secprops.Sstrong2);
            DA.SetData(8, secprops.Sweak1);
            DA.SetData(9, secprops.Sweak2);
            //DA.SetData(10, secprops.Zstrong);
            //DA.SetData(11, secprops.Zweak);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Properties;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F6CEB482-41D6-468B-B4E5-1F4FA37055E8"); }
        }
    }
}