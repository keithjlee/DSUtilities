using System;
using System.Collections.Generic;
using DSUtilities.Asap;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace DSUtilities.Asap_GH
{
    public class DeconstructElement : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DeconstructElement class.
        /// </summary>
        public DeconstructElement()
          : base("DeconstructElement", "DeconElem",
              "Deconstruct an Asap element",
              "DSutilities", "Asap")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Element", "Element", "Element to deconstruct", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("StartIndex", "iStart", "Start node index", GH_ParamAccess.item);
            pManager.AddIntegerParameter("EndIndex", "iEnd", "End node index", GH_ParamAccess.item);
            pManager.AddIntegerParameter("ElementID", "ElemID", "Ordered number ID of element", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Sec", "Cross section of element", GH_ParamAccess.item);
            pManager.AddBooleanParameter("JointRelease", "Release", "Joint releases in LCS (RxStart, RyStart, RzStart, RxEnd, RyEnd, RzEnd), true = released", GH_ParamAccess.list);
            pManager.AddPlaneParameter("LocalPlane", "XY", "Element orientation as defined by a plane", GH_ParamAccess.item);
            pManager.AddNumberParameter("Forces", "F", "Internal forces in LCS [Px1, Vy1, Vz1, Tx1, My1, Mz1, Px2, Vy2, Vz2, Tx2, My2, Mz2]", GH_ParamAccess.list);
            pManager.AddNumberParameter("AxialForce", "FAxial", "Axial force of element", GH_ParamAccess.item);
            pManager.AddTextParameter("ID", "ID", "User-defined element ID", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Element element = new Element();
            if (!DA.GetData(0, ref element)) return;

            DA.SetData(0, element.StartIndex);
            DA.SetData(1, element.EndIndex);
            DA.SetData(2, element.ElementID);
            DA.SetData(3, element.Section);
            DA.SetDataList(4, element.Release);

            Plane element_plane = new Plane(Point3d.Origin, element.XAxis, element.YAxis);

            DA.SetData(5, element_plane);
            DA.SetDataList(6, element.Forces);
            DA.SetData(7, element.AxialForce);
            DA.SetData(8, element.ID);
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
            get { return new Guid("3A7A3714-D7DB-458C-B8F1-AE33D3BEFD00"); }
        }
    }
}