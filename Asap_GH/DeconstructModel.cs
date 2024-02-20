using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DSUtilities.Asap;

namespace DSUtilities.Asap_GH
{
    public class DeconstructModel : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructModel class.
        /// </summary>
        public DeconstructModel()
          : base("DeconstructModel", "DeconModel",
              "Deconstruct an Asap model into its constituent parts",
              "DSutilities", "Asap")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "Model", "Asap model to deconstruct", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Nodes", "Nodes", "Nodes", GH_ParamAccess.list);
            pManager.AddGenericParameter("Elements", "Elements", "Elements", GH_ParamAccess.list);
            pManager.AddGenericParameter("Loads", "Loads", "Loads", GH_ParamAccess.list);
            pManager.AddIntegerParameter("FreeIndices", "iFree", "Indices of nodes that are fully free to displace", GH_ParamAccess.list);
            pManager.AddIntegerParameter("FixedIndices", "iFixed", "Indices of nodes that have one or more displacement constraints", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Model model = new Model();
            if (!DA.GetData(0, ref model)) return;

            DA.SetDataList(0, model.Nodes);
            DA.SetDataList(1, model.Elements);
            DA.SetDataList(2, model.Loads);
            DA.SetDataList(3, model.FreeIndices);
            DA.SetDataList(4, model.SupportIndices);
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
            get { return new Guid("E38AEA43-26E2-4C47-82F1-21A4AE028640"); }
        }
    }
}