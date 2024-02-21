using System;
using System.Collections.Generic;
using DSUtilities.Asap;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Asap_GH
{
    public class DeconstructNodeMoment : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructNodeMoment class.
        /// </summary>
        public DeconstructNodeMoment()
          : base("DeconstructNodeMoment", "DeconNodeMoment",
              "Deconstruct a nodal moment",
              "DSutilities", "Asap")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("NodeMoment", "NodeMoment", "Node moment to deconstruct", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("NodeIndex", "iNode", "Index of node that load is applied to", GH_ParamAccess.item);
            pManager.AddVectorParameter("Moment", "Moment", "Moment vector", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GHnodemoment load = new GHnodemoment();
            if (!DA.GetData(0, ref load)) return;

            Vector3d moment = new Vector3d(load.value[0], load.value[1], load.value[2]);

            DA.SetData(0, load.iNode);
            DA.SetData(1, moment);
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
            get { return new Guid("0CC57ECE-2039-48FF-8397-5EF145CE0EC7"); }
        }
    }
}