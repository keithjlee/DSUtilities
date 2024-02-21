using System;
using System.Collections.Generic;
using DSUtilities.Asap;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Asap_GH
{
    public class DeconstructNodeForce : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructNodeForce class.
        /// </summary>
        public DeconstructNodeForce()
          : base("DeconstructNodeForce", "DeconNodeForce",
              "Deconstruct a nodal force",
              "DSutilities", "Asap")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("NodeForce", "NodeForce", "Node force to deconstruct", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("NodeIndex", "iNode", "Index of node that load is applied to", GH_ParamAccess.item);
            pManager.AddVectorParameter("Load", "Load", "Load vector", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GHnodeforce load = new GHnodeforce();
            if (!DA.GetData(0, ref load)) return;

            Vector3d force = new Vector3d(load.value[0], load.value[1], load.value[2]);

            DA.SetData(0, load.iNode);
            DA.SetData(1, force);
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
            get { return new Guid("36B6D4E2-BDC5-4136-A96B-276ED8172AB1"); }
        }
    }
}