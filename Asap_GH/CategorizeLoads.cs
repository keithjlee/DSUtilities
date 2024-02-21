using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DSUtilities.Asap;
using System.Linq;

namespace DSUtilities.Asap_GH
{
    public class CategorizeLoads : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CategorizeLoads class.
        /// </summary>
        public CategorizeLoads()
          : base("CategorizeLoads", "LoadCats",
              "Split a collection of loads into load types",
              "DSutilities", "Asap")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Loads", "Loads", "Loads to categorize", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("NodeForces", "NodeForce", "Node force loads", GH_ParamAccess.list);
            pManager.AddGenericParameter("NodeMoments", "NodeMoment", "Node moment loads", GH_ParamAccess.list);
            pManager.AddGenericParameter("LineLoads", "LineLoad", "Element line loads", GH_ParamAccess.list);
            pManager.AddGenericParameter("PointLoads", "PointLoad", "Element point loads", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GHload> loads = new List<GHload>();
            DA.GetDataList(0, loads);

            List<GHnodeforce> nodeforces = loads.OfType<GHnodeforce>().ToList();
            List<GHnodemoment> nodemoments = loads.OfType<GHnodemoment>().ToList();
            List<GHlineload> lineloads = loads.OfType<GHlineload>().ToList();
            List<GHpointload> pointloads = loads.OfType<GHpointload>().ToList();

            DA.SetDataList(0, nodeforces);
            DA.SetDataList(1, nodemoments);
            DA.SetDataList(2 , lineloads);
            DA.SetDataList(3 , pointloads);
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
            get { return new Guid("6009F735-4997-4B7A-9B05-B1DEC04E6651"); }
        }
    }
}