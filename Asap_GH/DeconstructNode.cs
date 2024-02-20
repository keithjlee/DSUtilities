using System;
using System.Collections.Generic;
using DSUtilities.Asap;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Asap_GH
{
    public class DeconstructNode : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructNode class.
        /// </summary>
        public DeconstructNode()
          : base("DeconstructNode", "DeconNode",
              "Deconstruct an Asap node",
              "DSutilities", "Asap")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "Node", "Node to deconstruct", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Position", "Pos", "Position of node", GH_ParamAccess.item);
            pManager.AddBooleanParameter("DegreesOfFreedom", "DOFs", "Degrees of freedom of node (true: free; false: fixed)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("NodeID", "NodeID", "Ordered number ID of node", GH_ParamAccess.item);
            pManager.AddNumberParameter("Reaction", "Rxn", "Reaction vector at node (if applicable)", GH_ParamAccess.list);
            pManager.AddVectorParameter("ForceReaction", "RxnForce", "Force reaction vector at node (if applicable)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Displacement", "U", "Full displacement vector at node", GH_ParamAccess.list);
            pManager.AddVectorParameter("SpatialDisplacement", "Uxyz", "Translational displacement vector of node", GH_ParamAccess.item);
            pManager.AddTextParameter("ID", "ID", "User-defined node ID", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Node node = new Node();
            if (!DA.GetData(0, ref node)) return;

            DA.SetData(0, node.Position);
            DA.SetDataList(1, node.DOF);
            DA.SetData(2, node.NodeID);
            DA.SetDataList(3, node.Reaction);
            DA.SetData(4, node.ForceReaction);
            DA.SetDataList(5, node.U);
            DA.SetData(6, node.Displacement);
            DA.SetData(7, node.ID);
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
            get { return new Guid("B040995C-4E0F-4B60-9475-506D241C076F"); }
        }
    }
}